<script lang="ts">
  import { enhance } from '$app/forms';
  import type { PageData } from './$types';
  let { data }: { data: PageData } = $props();

  let editingDate = $state<string | null>(null);
</script>

<svelte:head><title>Meals – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">Meal planner</div>
      <div class="page-subtitle">Weekly dinner overview</div>
    </div>
    <div class="flex gap-1">
      <a href="?week={data.weekOffset - 1}" class="btn btn-secondary btn-sm">‹ Prev</a>
      <a href="?week=0" class="btn btn-secondary btn-sm">This week</a>
      <a href="?week={data.weekOffset + 1}" class="btn btn-secondary btn-sm">Next ›</a>
    </div>
  </div>

  <div class="week-grid">
    {#each data.weekDays as day}
      <div class="week-day-card" class:today={day.isToday}>
        <div class="week-day-header">
          <div class="week-day-name">{day.dayName}</div>
          <div class="week-day-date">{day.dayNum}</div>
        </div>
        <div class="week-day-body">
          {#if editingDate === day.date}
            <!-- Edit form -->
            <form
              method="POST"
              action="?/setMeal&week={data.weekOffset}"
              use:enhance={() => {
                return async ({ update }) => {
                  await update();
                  editingDate = null;
                };
              }}
            >
              <input type="hidden" name="date" value={day.date} />
              <div class="form-group" style="margin-bottom:0.5rem">
                <select name="recipe_id" style="font-size:0.8rem;padding:0.35rem 0.5rem">
                  <option value="">— no recipe —</option>
                  {#each data.recipes as r}
                    <option value={r.id}>{r.title}</option>
                  {/each}
                </select>
              </div>
              <div class="form-group" style="margin-bottom:0.5rem">
                <input
                  name="custom_meal"
                  type="text"
                  placeholder="Or type meal name…"
                  value={day.meal?.custom_meal ?? ''}
                  style="font-size:0.8rem;padding:0.35rem 0.5rem"
                />
              </div>
              <div class="form-group" style="margin-bottom:0.5rem">
                <input
                  name="notes"
                  type="text"
                  placeholder="Notes…"
                  value={day.meal?.notes ?? ''}
                  style="font-size:0.8rem;padding:0.35rem 0.5rem"
                />
              </div>
              <div class="flex gap-1">
                <button type="submit" class="btn btn-primary btn-sm" style="font-size:0.75rem;padding:0.25rem 0.5rem">Save</button>
                <button type="button" class="btn btn-ghost btn-sm" style="font-size:0.75rem" onclick={() => (editingDate = null)}>✕</button>
              </div>
            </form>
          {:else if day.meal}
            <div class="meal-name">
              {day.meal.recipe_title ?? day.meal.custom_meal}
            </div>
            {#if day.meal.recipe_id}
              <a href="/recipes/{day.meal.recipe_id}" class="meal-recipe-link">View recipe →</a>
            {/if}
            {#if day.meal.notes}
              <div class="text-xs text-muted mt-1">{day.meal.notes}</div>
            {/if}
            <div class="flex gap-1" style="margin-top:0.5rem">
              <button class="btn btn-ghost btn-sm" style="font-size:0.72rem;padding:0.2rem 0.4rem" onclick={() => (editingDate = day.date)}>Edit</button>
              <form method="POST" action="?/clearMeal&week={data.weekOffset}" use:enhance>
                <input type="hidden" name="date" value={day.date} />
                <button type="submit" class="btn btn-danger btn-sm" style="font-size:0.72rem;padding:0.2rem 0.4rem">Clear</button>
              </form>
            </div>
          {:else}
            <div class="no-meal">Not planned</div>
            <button class="btn btn-ghost btn-sm" style="margin-top:0.5rem;font-size:0.75rem;padding:0.25rem 0.5rem" onclick={() => (editingDate = day.date)}>+ Plan</button>
          {/if}
        </div>
      </div>
    {/each}
  </div>

  <div style="margin-top:1.5rem;text-align:right">
    <a href="/recipes" class="btn btn-secondary btn-sm">📖 Browse recipes</a>
    <a href="/recipes/new" class="btn btn-primary btn-sm" style="margin-left:0.5rem">+ New recipe</a>
  </div>
</div>
