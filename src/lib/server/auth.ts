import { db } from './db';
import bcrypt from 'bcryptjs';
import { randomUUID } from 'crypto';

const SESSION_TTL = 30 * 24 * 60 * 60; // 30 days in seconds

export interface User {
  id: number;
  username: string;
  display_name: string;
  color: string;
}

export function verifyPassword(username: string, password: string): User | null {
  const row = db
    .prepare(
      'SELECT id, username, password_hash, display_name, color FROM users WHERE username = ?'
    )
    .get(username) as
    | { id: number; username: string; password_hash: string; display_name: string; color: string }
    | undefined;

  if (!row || !bcrypt.compareSync(password, row.password_hash)) return null;

  return { id: row.id, username: row.username, display_name: row.display_name, color: row.color };
}

export function createSession(userId: number): string {
  const id = randomUUID();
  const expiresAt = Math.floor(Date.now() / 1000) + SESSION_TTL;
  db.prepare('INSERT INTO sessions (id, user_id, expires_at) VALUES (?, ?, ?)').run(
    id,
    userId,
    expiresAt
  );
  return id;
}

export function getSession(sessionId: string): User | null {
  const row = db
    .prepare(
      `SELECT s.user_id as id, u.username, u.display_name, u.color
       FROM sessions s
       JOIN users u ON u.id = s.user_id
       WHERE s.id = ? AND s.expires_at > strftime('%s','now')`
    )
    .get(sessionId) as User | undefined;
  return row ?? null;
}

export function deleteSession(sessionId: string): void {
  db.prepare('DELETE FROM sessions WHERE id = ?').run(sessionId);
}

export function getUsers(): User[] {
  return db.prepare('SELECT id, username, display_name, color FROM users').all() as User[];
}
