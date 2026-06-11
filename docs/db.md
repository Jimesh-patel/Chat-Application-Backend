```
-- ==========================================
-- MARTEN INTERNAL (AUTO MANAGED)
-- ==========================================

mt_events
mt_streams
mt_event_progression
mt_doc_*


-- ==========================================
-- IDENTITY MODULE
-- ==========================================

users
-----
id UUID PK
email VARCHAR(255) UNIQUE
username VARCHAR(100) UNIQUE
display_name VARCHAR(150)
profile_picture_url TEXT
status VARCHAR(50)
created_at TIMESTAMPTZ
updated_at TIMESTAMPTZ


user_sessions
-------------
id UUID PK
user_id UUID
refresh_token_hash TEXT
device_name VARCHAR(200)
device_id VARCHAR(200)
ip_address VARCHAR(100)
user_agent TEXT
expires_at TIMESTAMPTZ
revoked_at TIMESTAMPTZ NULL
created_at TIMESTAMPTZ


blocked_users
-------------
id UUID PK
user_id UUID
blocked_user_id UUID
created_at TIMESTAMPTZ


-- ==========================================
-- PRESENCE MODULE
-- ==========================================

user_presence
-------------
user_id UUID PK
is_online BOOLEAN
last_seen_at TIMESTAMPTZ
connection_count INT
updated_at TIMESTAMPTZ


user_connections
----------------
connection_id VARCHAR(200) PK
user_id UUID
server_node VARCHAR(200)
connected_at TIMESTAMPTZ
last_heartbeat_at TIMESTAMPTZ


-- ==========================================
-- CHAT MODULE
-- ==========================================

conversations
-------------
id UUID PK
type VARCHAR(20)
created_by UUID
last_message_id UUID NULL
last_message_at TIMESTAMPTZ NULL
created_at TIMESTAMPTZ


conversation_members
--------------------
conversation_id UUID
user_id UUID
joined_at TIMESTAMPTZ
muted_until TIMESTAMPTZ NULL
PRIMARY KEY(conversation_id, user_id)


messages
--------
id UUID PK
conversation_id UUID
sender_id UUID
content TEXT
message_type VARCHAR(50)
reply_to_message_id UUID NULL
attachment_count INT
is_edited BOOLEAN
is_deleted BOOLEAN
sent_at TIMESTAMPTZ
edited_at TIMESTAMPTZ NULL
deleted_at TIMESTAMPTZ NULL


message_receipts
----------------
message_id UUID
user_id UUID
delivered_at TIMESTAMPTZ NULL
read_at TIMESTAMPTZ NULL
PRIMARY KEY(message_id, user_id)


message_reactions
-----------------
message_id UUID
user_id UUID
reaction VARCHAR(50)
created_at TIMESTAMPTZ
PRIMARY KEY(message_id, user_id, reaction)


message_attachments
-------------------
id UUID PK
message_id UUID
file_name VARCHAR(500)
file_size BIGINT
content_type VARCHAR(200)
blob_url TEXT
uploaded_at TIMESTAMPTZ


pinned_messages
---------------
conversation_id UUID
message_id UUID
pinned_by UUID
pinned_at TIMESTAMPTZ
PRIMARY KEY(conversation_id, message_id)


-- ==========================================
-- GROUP MODULE
-- ==========================================

groups
------
id UUID PK
name VARCHAR(200)
description TEXT
avatar_url TEXT
created_by UUID
created_at TIMESTAMPTZ
updated_at TIMESTAMPTZ


group_members
-------------
group_id UUID
user_id UUID
role VARCHAR(50)
joined_at TIMESTAMPTZ
PRIMARY KEY(group_id, user_id)


group_invites
-------------
id UUID PK
group_id UUID
invited_user_id UUID
invited_by UUID
status VARCHAR(50)
created_at TIMESTAMPTZ
expires_at TIMESTAMPTZ


group_join_requests
-------------------
id UUID PK
group_id UUID
user_id UUID
status VARCHAR(50)
created_at TIMESTAMPTZ
resolved_at TIMESTAMPTZ NULL


-- ==========================================
-- NOTIFICATION MODULE
-- ==========================================

notifications
-------------
id UUID PK
user_id UUID
type VARCHAR(100)
title VARCHAR(250)
body TEXT
payload JSONB
is_read BOOLEAN
created_at TIMESTAMPTZ
read_at TIMESTAMPTZ NULL


notification_preferences
------------------------
user_id UUID PK
email_enabled BOOLEAN
push_enabled BOOLEAN
in_app_enabled BOOLEAN
group_notifications BOOLEAN
message_notifications BOOLEAN
updated_at TIMESTAMPTZ


-- ==========================================
-- EVENT DELIVERY
-- ==========================================

outbox_messages
---------------
id UUID PK
event_id UUID
event_type VARCHAR(500)
payload JSONB
occurred_at TIMESTAMPTZ
published_at TIMESTAMPTZ NULL
retry_count INT
status VARCHAR(50)


inbox_messages
--------------
id UUID PK
message_id UUID
consumer_name VARCHAR(200)
processed_at TIMESTAMPTZ
status VARCHAR(50)


-- ==========================================
-- FILE STORAGE
-- ==========================================

files
-----
id UUID PK
uploaded_by UUID
file_name VARCHAR(500)
content_type VARCHAR(200)
file_size BIGINT
blob_url TEXT
checksum VARCHAR(500)
uploaded_at TIMESTAMPTZ


-- ==========================================
-- AUDIT
-- ==========================================

audit_logs
----------
id UUID PK
user_id UUID NULL
action VARCHAR(200)
entity_type VARCHAR(200)
entity_id UUID
metadata JSONB
created_at TIMESTAMPTZ


-- ==========================================
-- INDEXES
-- ==========================================

messages(conversation_id, sent_at DESC)

message_receipts(user_id)

conversation_members(user_id)

group_members(user_id)

notifications(user_id, is_read)

user_connections(user_id)

outbox_messages(status)

inbox_messages(message_id, consumer_name)
```
