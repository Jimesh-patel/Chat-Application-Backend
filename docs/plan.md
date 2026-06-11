# Chat Application - Architecture & Implementation Plan

## 1. Problem Statement

Build a modern, scalable, event-driven chat platform using .NET and Azure that demonstrates enterprise-grade architecture patterns including:

- Domain Driven Design (DDD)
- CQRS
- Event Sourcing
- Modular Monolith
- Actor Model
- Distributed Systems
- Real-time Communication

The application should support direct messaging, group conversations, notifications, user presence tracking, and multiple authentication providers while remaining ready for future microservice extraction.

---

# 2. Functional Requirements

## Authentication & Identity

- User Registration
- Email + Password Login
- Google OAuth Login
- Logout
- Logout From All Devices
- Refresh Token Flow
- Change Password
- User Profile Management
- Session Management

## Chat

- One-to-One Messaging
- Message Delivery
- Message Read Status
- Message Editing
- Message Deletion
- Message Reactions
- Message Attachments
- Pinned Messages

## Groups

- Create Group
- Update Group
- Invite Members
- Join Requests
- Add Member
- Remove Member
- Promote Admin
- Leave Group

## Presence

- Online / Offline Status
- Last Seen
- Typing Indicators
- Multi-Device Presence

## Notifications

- In-App Notifications
- Push Notification Ready
- Notification Preferences

---

# 3. Non-Functional Requirements

- Event Driven Architecture
- Event Sourcing
- Horizontal Scalability
- High Availability
- Real-Time Communication
- Observability
- Distributed Tracing
- Secure Authentication
- Clean Module Boundaries
- Cloud Ready

---

# 4. Tech Stack

## Backend

- .NET 10
- ASP.NET Core Minimal APIs
- C#

## Event Sourcing

- Marten
- PostgreSQL

## Actor System

- Akka.NET
- Akka Cluster Sharding
- Akka Persistence

## Messaging

- Azure Service Bus

## Realtime

- SignalR

## Cache

- Redis

## Database

- PostgreSQL

## Cloud

- Azure App Service / Container Apps
- Azure PostgreSQL
- Azure Redis
- Azure Service Bus
- Azure Blob Storage
- Azure Key Vault
- Azure Application Insights

## Frontend

- React
- JavaScript

---

# 5. Architecture Style

## Architecture

Modular Monolith

Future-ready for Microservice extraction.

## Patterns

- Domain Driven Design
- CQRS
- Event Sourcing
- Vertical Slice Architecture
- Hexagonal Architecture
- Actor Model

---

# 6. Project Structure

src
├── Hosting
│ └── ChatApp.Host
│
├── Platform
│ ├── Platform.Common
│ ├── Platform.Contracts
│ ├── Platform.Persistence
│ ├── Platform.Messaging
│ ├── Platform.Auth
│ ├── Platform.Caching
│ ├── Platform.Http
│ ├── Platform.Observability
│ ├── Platform.Akka
│ └── Platform.Realtime
│
└── Modules
│
├── Identity
│ ├── Identity
│ └── Identity.Contracts
│
├── Chat
│ ├── Chat
│ └── Chat.Contracts
│
├── Groups
│ ├── Groups
│ └── Groups.Contracts
│
├── Notifications
│ ├── Notifications
│ └── Notifications.Contracts
│
└── Presence
├── Presence
└── Presence.Contracts

---

# 7. Module Responsibilities

## Identity

Responsible For:

- Registration
- Login
- OAuth
- Session Management
- Roles
- User Profile

## Chat

Responsible For:

- Conversations
- Messages
- Attachments
- Reactions
- Read Receipts

## Groups

Responsible For:

- Group Lifecycle
- Membership
- Roles
- Invitations

## Presence

Responsible For:

- Online Status
- Last Seen
- Typing Indicators

## Notifications

Responsible For:

- Notification Generation
- Delivery Tracking
- User Preferences

---

# 8. Authentication Strategy

## Supported Providers

### Local Authentication

Email + Password

### External Authentication

Google OAuth

## Authentication Flow

Both providers ultimately create and authenticate the same User aggregate.

The application always issues its own:

- Access Token (JWT)
- Refresh Token

Google access tokens are never used inside the system after authentication.

## Session Management

- Multi Device Login
- Refresh Tokens
- Session Revocation
- Logout All Devices

## Password Security

Password hashing via ASP.NET PasswordHasher.

---

# 9. Authorization Strategy

## RBAC

Roles:

- User
- Admin

Future:

- Moderator
- SuperAdmin

## Authorization Types

- Role-Based Authorization
- Policy-Based Authorization
- Resource-Based Authorization

---

# 10. Event Sourcing Strategy

Source Of Truth:

Events

Examples:

- UserRegistered
- UserLoggedIn
- SessionCreated
- ConversationCreated
- MessageSent
- MessageRead
- GroupCreated
- MemberAdded
- UserOnline

Event Store:

Marten Event Store

Database:

PostgreSQL

---

# 11. CQRS Strategy

## Command Side

Commands mutate state.

Examples:

- RegisterUser
- LoginUser
- SendMessage
- CreateGroup

Commands are processed by Aggregates and Actors.

## Query Side

Queries use projections only.

Examples:

- GetMessages
- GetConversation
- GetNotifications

Queries never read actors.

---

# 12. Actor System Design

## Actor Types

### ConversationActor

Responsibilities:

- Send Message
- Edit Message
- Delete Message
- Read Receipts

Entity Id:

ConversationId

### GroupActor

Responsibilities:

- Membership Changes
- Group Administration

Entity Id:

GroupId

### PresenceActor

Responsibilities:

- Online State
- Typing Indicators
- Last Seen

Entity Id:

UserId

## Cluster Strategy

Akka Cluster Sharding

Actors distributed automatically across nodes.

---

# 13. Realtime Design

Technology:

SignalR

Flow:

Client
→ SignalR Hub
→ Command
→ Actor
→ Event Store
→ Projection
→ SignalR Push

---

# 14. Database Strategy

## Event Store

Managed by Marten

- mt_events
- mt_streams
- mt_event_progression

## Read Models

- users
- user_presence
- conversations
- conversation_members
- messages
- message_receipts
- groups
- group_members
- notifications

## Infrastructure Tables

- user_sessions
- external_identities
- user_connections
- outbox_messages
- inbox_messages
- files
- audit_logs

---

# 15. Integration Strategy

## Internal Communication

Akka.NET

## Module Communication

Integration Events

## External Communication

Azure Service Bus

---

# 16. Observability

- OpenTelemetry
- Structured Logging by Serilog
- Distributed Tracing
- Application Insights

---

# 17. Cloud Architecture

Azure Services:

- Azure PostgreSQL
- Azure Service Bus
- Azure Redis
- Azure Blob Storage
- Azure Key Vault
- Azure Application Insights
- Azure Container Apps

---
