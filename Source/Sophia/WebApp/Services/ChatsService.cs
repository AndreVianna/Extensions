﻿using System.Linq.Expressions;

namespace Sophia.WebApp.Services;

public class ChatsService(ApplicationDbContext dbContext)
    : IChatsService {
    public async Task<IReadOnlyList<ChatData>> GetList(string? filter = null) {
        var filterClause = BuildFilter(filter);
        return await dbContext.Chats
                              .Include(c => c.Persona)
                              .Include(c => c.Messages)
                              .AsNoTracking()
                              .Where(filterClause)
                              .Select(s => s.ToDto())
                              .ToArrayAsync();
    }

    private static Expression<Func<ChatEntity, bool>> BuildFilter(string? filter)
        => filter switch {
            "ShowArchived" => (_) => true,
            _ => c => c.IsActive == true,
        };

    public async Task<ChatData?> GetById(string id) {
        var entity = await dbContext.Chats
                                    .Include(c => c.Persona)
                                    .Include(c => c.Messages)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(s => s.Id == id);
        return entity?.ToDto();
    }

    public async Task Create(ChatData chat) {
        var entity = chat.ToEntity();
        dbContext.Chats.Add(entity);
        await dbContext.SaveChangesAsync();
        chat.Id = entity.Id;
    }

    public async Task Archive(string id) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.IsActive = false;
        await dbContext.SaveChangesAsync();
    }

    public async Task Unarchive(string id) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.IsActive = true;
        await dbContext.SaveChangesAsync();
    }

    public async Task Rename(string id, string newName) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.Title = newName;
        await dbContext.SaveChangesAsync();
    }

    public async Task AddMessage(string id, MessageData message) {
        var entity = await dbContext.Chats
                                    .Include(c => c.Messages)
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.Messages.Add(message.ToEntity(entity));
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(string id) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity is null) return;
        dbContext.Chats.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}