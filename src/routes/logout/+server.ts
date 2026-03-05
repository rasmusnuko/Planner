import { redirect } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import { deleteSession } from '$lib/server/auth';

export const POST: RequestHandler = async ({ cookies }) => {
  const sessionId = cookies.get('session');
  if (sessionId) {
    deleteSession(sessionId);
    cookies.delete('session', { path: '/' });
  }
  redirect(303, '/login');
};
