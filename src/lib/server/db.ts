import Database from 'better-sqlite3';
import bcrypt from 'bcryptjs';
import fs from 'fs';
import path from 'path';

const DB_PATH = process.env.DATABASE_PATH || './data/planner.db';

// Ensure the data directory exists
const dbDir = path.dirname(DB_PATH);
if (!fs.existsSync(dbDir)) {
  fs.mkdirSync(dbDir, { recursive: true });
}

export const db = new Database(DB_PATH);

db.pragma('journal_mode = WAL');
db.pragma('foreign_keys = ON');

// ─── Schema ───────────────────────────────────────────────────────────────────

db.exec(`
  CREATE TABLE IF NOT EXISTS users (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    username      TEXT    UNIQUE NOT NULL,
    password_hash TEXT    NOT NULL,
    display_name  TEXT    NOT NULL,
    color         TEXT    NOT NULL DEFAULT '#4f46e5'
  );

  CREATE TABLE IF NOT EXISTS sessions (
    id         TEXT    PRIMARY KEY,
    user_id    INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    expires_at INTEGER NOT NULL
  );

  CREATE TABLE IF NOT EXISTS events (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    title       TEXT    NOT NULL,
    date        TEXT    NOT NULL,
    type        TEXT    NOT NULL DEFAULT 'general',
    assigned_to INTEGER REFERENCES users(id) ON DELETE SET NULL,
    notes       TEXT,
    created_by  INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at  INTEGER NOT NULL DEFAULT (strftime('%s','now'))
  );

  CREATE TABLE IF NOT EXISTS shopping_items (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    name       TEXT    NOT NULL,
    category   TEXT    NOT NULL DEFAULT 'other',
    checked    INTEGER NOT NULL DEFAULT 0,
    added_by   INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at INTEGER NOT NULL DEFAULT (strftime('%s','now'))
  );

  CREATE TABLE IF NOT EXISTS todo_lists (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    name       TEXT    NOT NULL,
    owner_id   INTEGER REFERENCES users(id) ON DELETE CASCADE,
    is_shared  INTEGER NOT NULL DEFAULT 0,
    created_at INTEGER NOT NULL DEFAULT (strftime('%s','now'))
  );

  CREATE TABLE IF NOT EXISTS todo_items (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    list_id     INTEGER NOT NULL REFERENCES todo_lists(id) ON DELETE CASCADE,
    text        TEXT    NOT NULL,
    done        INTEGER NOT NULL DEFAULT 0,
    assigned_to INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at  INTEGER NOT NULL DEFAULT (strftime('%s','now'))
  );

  CREATE TABLE IF NOT EXISTS recipes (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    title        TEXT    NOT NULL,
    description  TEXT,
    ingredients  TEXT    NOT NULL DEFAULT '[]',
    instructions TEXT,
    prep_time    INTEGER,
    servings     INTEGER,
    created_by   INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at   INTEGER NOT NULL DEFAULT (strftime('%s','now'))
  );

  CREATE TABLE IF NOT EXISTS meal_plan (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    date        TEXT    NOT NULL UNIQUE,
    recipe_id   INTEGER REFERENCES recipes(id) ON DELETE SET NULL,
    custom_meal TEXT,
    notes       TEXT
  );

  CREATE TABLE IF NOT EXISTS milestones (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    date        TEXT    NOT NULL,
    title       TEXT    NOT NULL,
    description TEXT,
    emoji       TEXT    NOT NULL DEFAULT '⭐',
    created_by  INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at  INTEGER NOT NULL DEFAULT (strftime('%s','now'))
  );
`);

// ─── Seed initial users from environment variables ────────────────────────────

function seedUsers() {
  const count = (db.prepare('SELECT COUNT(*) as c FROM users').get() as { c: number }).c;
  if (count > 0) return;

  const insert = db.prepare(`
    INSERT INTO users (username, password_hash, display_name, color)
    VALUES (?, ?, ?, ?)
  `);

  const u1 = {
    username: process.env.USER1_USERNAME || 'user1',
    password: process.env.USER1_PASSWORD || 'password1',
    display_name: process.env.USER1_DISPLAY_NAME || 'User 1',
    color: process.env.USER1_COLOR || '#4f46e5'
  };

  const u2 = {
    username: process.env.USER2_USERNAME || 'user2',
    password: process.env.USER2_PASSWORD || 'password2',
    display_name: process.env.USER2_DISPLAY_NAME || 'User 2',
    color: process.env.USER2_COLOR || '#ec4899'
  };

  insert.run(u1.username, bcrypt.hashSync(u1.password, 10), u1.display_name, u1.color);
  insert.run(u2.username, bcrypt.hashSync(u2.password, 10), u2.display_name, u2.color);

  console.log(`[planner] Created users: ${u1.username}, ${u2.username}`);
}

seedUsers();
