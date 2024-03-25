﻿namespace Sophia.WebClient.Pages.Chats;

public partial class ChatsPage {
    private IReadOnlyList<ChatData> _chats = [];
    private bool _showChatSetupDialog;
    private ChatData? _selectedChat;
    private bool _showDeleteConfirmationDialog;

    private bool _showArchived;
    private string? _renamingChatId;
    private string _newChatName = string.Empty;

    [Inject] public required IChatsRemoteService ChatsService { get; set; }

    [Inject] public required NavigationManager NavigationManager { get; set; }

    protected override Task OnInitializedAsync()
        => Load();

    private async Task Load() {
        _chats = await ChatsService.GetList(_showArchived ? "ShowArchived" : null);
        StateHasChanged();
    }

    private void Start() {
        _selectedChat = new();
        _showChatSetupDialog = true;
    }

    private async Task StartChat() {
        if (_selectedChat is not null) {
            if (_selectedChat.Id is null) await ChatsService.Create(_selectedChat);
            NavigationManager.NavigateTo($"/chat/{_selectedChat.Id}");
        }
        CloseChatDialog();
    }

    private void CloseChatDialog() {
        _showChatSetupDialog = false;
        _selectedChat = new();
    }

    private void Resume(string chatId)
        => NavigationManager.NavigateTo($"/chat/{chatId}");

    private async Task Archive(string chatId) {
        await ChatsService.Archive(chatId);
        _chats = await ChatsService.GetList(_showArchived ? "ShowArchived" : null);
    }

    private async Task Unarchive(string chatId) {
        await ChatsService.Unarchive(chatId);
        _chats = await ChatsService.GetList(_showArchived ? "ShowArchived" : null);
    }

    private void Delete(ChatData chat) {
        _selectedChat = chat;
        _showDeleteConfirmationDialog = true;
    }

    private void CancelDelete() {
        _selectedChat = null;
        _showDeleteConfirmationDialog = false;
    }

    private async Task ExecuteDelete() {
        if (_selectedChat is not null) {
            await ChatsService.Delete(_selectedChat.Id!);
            _chats = await ChatsService.GetList(_showArchived ? "ShowArchived" : null);
            _selectedChat = null;
        }

        _showDeleteConfirmationDialog = false;
    }

    private void StartRename(string chatId, string currentName) {
        _renamingChatId = chatId;
        _newChatName = currentName;
    }

    private async Task ConfirmRename(string chatId) {
        await ChatsService.Rename(chatId, _newChatName);
        _chats = await ChatsService.GetList(_showArchived ? "ShowArchived" : null);
        CancelRename();
    }

    private void CancelRename() {
        _renamingChatId = null;
        _newChatName = string.Empty;
    }
}