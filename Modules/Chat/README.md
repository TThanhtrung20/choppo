# Chat module

The chat feature is being prepared as an isolated module. Its domain models currently describe conversations and messages only; they are not wired into the database, UI, or an AI provider.

## Structure

- `Models/ChatConversation.cs` — conversation/session metadata and message collection.
- `Models/ChatMessage.cs` — one message belonging to a conversation.
- `Models/ChatConversationStatus.cs` — active or archived conversation state.
- `Models/ChatMessageRole.cs` — user, assistant, or system role.

## Current scope

This is the model-only foundation requested for the AI chat feature. No API key, provider SDK, network call, database table, or migration is included yet. The `System` role exists as a future integration concept and should only be assigned by trusted server-side code, never by client-submitted content.
