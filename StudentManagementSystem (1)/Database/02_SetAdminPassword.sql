-- ============================================================
-- Run this AFTER you've generated a BCrypt hash for your password.
-- See README.md "Setting the Admin Password" section for how
-- to generate the hash quickly using an online BCrypt generator
-- or a tiny C# snippet.
-- ============================================================

USE StudentManagementDB;
GO

-- Replace 'PASTE_YOUR_BCRYPT_HASH_HERE' with the generated hash
UPDATE Users
SET PasswordHash = 'PASTE_YOUR_BCRYPT_HASH_HERE'
WHERE Username = 'admin';
GO

SELECT * FROM Users;
GO
