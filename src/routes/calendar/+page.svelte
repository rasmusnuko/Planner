<script lang="ts">
  import { enhance } from '$app/forms';
  import type { PageData } from './$types';

  let { data }: { data: PageData } = $props();

  // Group events by date for quick lookup
  const eventsByDate = $derived(() => {
    const map: Record<string, typeof data.events> = {};
    for (const e of data.events) {
      if (!map[e.date]) map[e.date] = [];
      map[e.date].push(e);
    }
    return map;
  });

  const weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
</script>

<svelte:head><title>Calendar – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">Calendar</div>
      <div class="page-subtitle">{data.monthName} {data.year}</div>
    </div>
    <div class="flex gap-1">
      <a href="?month={data.prevMonth}" class="btn btn-secondary btn-sm">‹ Prev</a>
      <a href="?month={data.monthParam}" class="btn btn-secondary btn-sm">Today</a>
      <a href="?month={data.nextMonth}" class="btn btn-secondary btn-sm">Next ›</a>
    </div>
  </div>

  <!-- Calendar grid -->
  <div class="calendar-grid" style="margin-bottom:1.5rem">
    {#each weekDays as d}
      <div class="calendar-day-header">{d}</div>
    {/each}

    {#each data.calendarDays as cell}
      {@const cellEvents = eventsByDate()[cell.date] ?? []}
      <a
        href="?month={data.monthParam}&date={cell.date}"
        class="calendar-cell"
        class:other-month={!cell.isCurrentMonth}
        class:today={cell.date === data.today}
        class:selected={cell.date === data.selectedDate}
      >
        <div class="cell-day">{cell.day}</div>
        <div class="cell-events">
          {#each cellEvents.slice(0, 3) as event}
            <div class="cell-event {event.type}">{event.title}</div>
          {/each}
          {#if cellEvents.length > 3}
            <div class="cell-event general">+{cellEvents.length - 3} more</div>
          {/if}
        </div>
      </a>
    {/each}
  </div>

  <!-- Selected day panel -->
  {#if data.selectedDate}
    <div class="card">
      <div class="card-header">
        <span class="card-title">📅 {data.selectedDate}</span>
        <a href="?month={data.monthParam}" class="btn btn-ghost btn-sm">✕ Close</a>
      </div>
      <div class="card-body">
        <!-- Events list -->
        {#if data.selectedDateEvents.length > 0}
          <div class="item-list mb-2">
            {#each data.selectedDateEvents as event}
              <div class="check-item">
                <div style="flex:1">
                  <span class="badge badge-{event.type}" style="margin-right:0.4rem"
                    >{event.type}</span
                  >
                  <strong>{event.title}</strong>
                  {#if event.assigned_name}
                    <span class="text-muted text-sm" style="margin-left:0.4rem">
                      → <span style="color:{event.assigned_color}">{event.assigned_name}</span>
                    </span>
                  {/if}
                  {#if event.notes}
                    <div class="text-sm text-muted mt-1">{event.notes}</div>
                  {/if}
                </div>
                <form method="POST" action="?/deleteEvent" use:enhance>
                  <input type="hidden" name="id" value={event.id} />
                  <button type="submit" class="btn btn-danger btn-icon btn-sm" title="Delete">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                      ><path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0"
                      /></svg
                    >
                  </button>
                </form>
              </div>
            {/each}
          </div>
        {:else}
          <p class="text-muted text-sm mb-2">No events on this day.</p>
        {/if}

        <!-- Add event form -->
        <form
          method="POST"
          action="?/addEvent&month={data.monthParam}&date={data.selectedDate}"
          use:enhance
          style="border-top:1px solid var(--border);padding-top:1rem"
        >
          <input type="hidden" name="date" value={data.selectedDate} />
          <div class="form-row" style="flex-wrap:wrap">
            <div class="form-group" style="flex:2;min-width:140px">
              <label for="title">Event title</label>
              <input id="title" name="title" type="text" placeholder="e.g. Pick up Lily" required />
            </div>
            <div class="form-group" style="min-width:120px">
              <label for="type">Type</label>
              <select id="type" name="type">
                <option value="general">General</option>
                <option value="pickup">Pickup</option>
                <option value="dinner">Dinner</option>
              </select>
            </div>
            <div class="form-group" style="min-width:120px">
              <label for="assigned_to">Assigned to</label>
              <select id="assigned_to" name="assigned_to">
                <option value="">Anyone</option>
                {#each data.users as u}
                  <option value={u.id}>{u.display_name}</option>
                {/each}
              </select>
            </div>
          </div>
          <div class="form-group">
            <label for="notes">Notes (optional)</label>
            <input id="notes" name="notes" type="text" placeholder="Any extra details…" />
          </div>
          <button type="submit" class="btn btn-primary btn-sm">Add event</button>
        </form>
      </div>
    </div>
  {:else}
    <p class="text-muted text-sm text-center">Click a day to view or add events.</p>
  {/if}
</div>
