import { getSession } from '$lib/server/auth';
import type { Handle } from '@sveltejs/kit';

export const handle: Handle = async ({ event, resolve }) => {
  const sessionId = event.cookies.get('session');
  event.locals.user = sessionId ? getSession(sessionId) : null;
  return resolve(event);
};
