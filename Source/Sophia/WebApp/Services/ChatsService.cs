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

    private Expression<Func<ChatEntity, bool>> BuildFilter(string? filter)
        => filter switch {
            "ShowArchived" => (_) => true,
            _ => c => c.IsActive == true,
        };

    public async Task<ChatData?> GetById(int id) {
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

    public async Task Archive(int id) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.IsActive = false;
        await dbContext.SaveChangesAsync();
    }

    public async Task Unarchive(int id) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.IsActive = true;
        await dbContext.SaveChangesAsync();
    }

    public async Task Rename(int id, string newName) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.Title = newName;
        await dbContext.SaveChangesAsync();
    }

    public async Task AddMessage(int id, MessageData message) {
        var entity = await dbContext.Chats
                                    .Include(c => c.Messages)
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity == null) return;
        entity.Messages.Add(message.ToEntity(entity));
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id) {
        var entity = await dbContext.Chats
                                    .FirstOrDefaultAsync(s => s.Id == id);
        if (entity is null) return;
        dbContext.Chats.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
