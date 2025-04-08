using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EMS.Infrastructure.Services;

public class EventWriteService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<EventWriteService> logger) : IEventWriteService
{

    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<EventWriteService> _logger = logger;


    private async Task<string> SaveImage(IFormFile image)
    {
        var request = _httpContextAccessor.HttpContext.Request;

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsFolder))   Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
            await image.CopyToAsync(fileStream);

        return $"{request.Scheme}://{request.Host}/uploads/{uniqueFileName}";
    }
    private string? GetUserId() =>
    _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


    public async Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, CancellationToken cancellationToken)
    {
        if (createEventDto.Image == null || createEventDto.Image.Length == 0)
            throw new ArgumentException("Image file is required");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(createEventDto.Image.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type. Allowed types: JPG, JPEG, PNG");

        if (createEventDto.Image.Length > 5 * 1024 * 1024) // 5 MB
            throw new ArgumentException("File size exceeds 5MB limit");

        var image = await SaveImage(createEventDto.Image);
        var newEvent = _mapper.Map<Event>(createEventDto);
        newEvent.Image = image;

        await _context.Events.AddAsync(newEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _context.EventOwners.AddAsync(new EventOwner { EventId = newEvent.EventId, UserId = GetUserId() }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(newEvent);
    }


    public async Task<EventDto?> ModifyEventAsync(EventCrudDto eventCrudDto, int eventId, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

        if (eventCrudDto.Image != null) existingEvent.Image = await SaveImage(eventCrudDto.Image);

        existingEvent.Name = eventCrudDto.Name ?? existingEvent.Name;
        existingEvent.Date = eventCrudDto.Date;
        existingEvent.Location = eventCrudDto.Location ?? existingEvent.Location;
        existingEvent.Description = eventCrudDto.Description ?? existingEvent.Description;
        existingEvent.Category = Enum.Parse<EventCategory>(eventCrudDto.Category);

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = user.IsInRole("Admin");
        var isOwner = await _context.EventOwners.AnyAsync(eo => eo.EventId == eventId && eo.UserId == userId, cancellationToken);

        if (!isOwner && !isAdmin) { return null; }

        _mapper.Map(eventCrudDto, existingEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(existingEvent);
    }


    public async Task<EventDto?> DeleteEventAsync(int eventId, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) {  return null;  }
        
        var eventDelete = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);
        if (eventDelete == null) {  return null;  }

        var isAdmin = user.IsInRole("Admin");
        var isOwner = await _context.EventOwners.AnyAsync(eo => eo.EventId == eventId && eo.UserId == userId, cancellationToken);
        if (!isOwner && !isAdmin) { return null; }

        _context.Events.Remove(eventDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(eventDelete);
    }



    public async Task<SignUpResult> SignUpForEventAsync(int eventId, string userId, CancellationToken cancellationToken)
    {
        var eventEntity = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

        if (eventEntity == null) return SignUpResult.EventNotFound;

        var existingRegistration = await _context.EventRegistrations
            .FirstOrDefaultAsync(er => er.EventId == eventId && er.UserId == userId, cancellationToken);

        if (existingRegistration != null) return SignUpResult.AlreadySignedUp;

        var eventRegistration = new EventRegistration
        {
            EventId = eventId,
            UserId = userId
        };

        await _context.EventRegistrations.AddAsync(eventRegistration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return SignUpResult.Success;
    }



    public async Task<ReviewDto?> AddReviewAsync(ReviewDto reviewDto, CancellationToken cancellationToken)
    {

        if (reviewDto == null)
            throw new ArgumentNullException(nameof(reviewDto));

        var eventEntity = await _context.Events
            .Include(e => e.EventReviews)
            .FirstOrDefaultAsync(e => e.EventId == reviewDto.EventId, cancellationToken);

        if (eventEntity == null)
            throw new KeyNotFoundException($"Event with ID {reviewDto.EventId} not found");

        var existingReview = await _context.EventReviews
            .FirstOrDefaultAsync(r =>
                r.EventId == reviewDto.EventId &&
                r.UserId == reviewDto.UserId,
                cancellationToken );

        if (existingReview != null)
            throw new InvalidOperationException("User has already reviewed this event");

        var newReview = new EventReview
        {
            EventReviewId = reviewDto.EventReviewId,
            EventId = reviewDto.EventId,
            UserId = reviewDto.UserId,
            RatingStars = reviewDto.RatingStars,
            ReviewText = reviewDto.ReviewText,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _context.EventReviews.AddAsync(newReview, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await UpdateEventReviewStats(reviewDto.EventId, cancellationToken);

            return new ReviewDto
            {
                EventReviewId = newReview.EventReviewId,
                EventId = newReview.EventId,
                UserId = newReview.UserId,
                RatingStars = newReview.RatingStars,
                ReviewText = newReview.ReviewText,
                CreatedAt = newReview.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding review");
            throw new InvalidOperationException("User has already reviewed this event");
        }
    }

    private async Task UpdateEventReviewStats(int eventId, CancellationToken cancellationToken)
    {
        var stats = await _context.EventReviews
            .Where(r => r.EventId == eventId)
            .GroupBy(r => r.EventId)
            .Select(g => new
            {
                Count = g.Count(),
                Average = g.Average(r => r.RatingStars)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (stats != null)
        {
            var eventEntity = await _context.Events
                .FirstAsync(e => e.EventId == eventId, cancellationToken);

            eventEntity.ReviewsCount = stats.Count;
            eventEntity.AverageRating = stats.Average;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
