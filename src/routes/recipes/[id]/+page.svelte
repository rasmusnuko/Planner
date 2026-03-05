<script lang="ts">
  import { enhance } from '$app/forms';
  import type { PageData, ActionData } from './$types';

  let { data, form }: { data: PageData; form: ActionData } = $props();
  let editing = $state(false);

  const r = $derived(data.recipe);
</script>

<svelte:head><title>{data.recipe.title} – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">{r.title}</div>
      {#if r.creator_name}
        <div class="page-subtitle">By {r.creator_name}</div>
      {/if}
    </div>
    <div class="flex gap-1">
      <a href="/recipes" class="btn btn-ghost btn-sm">← Back</a>
      <button class="btn btn-secondary btn-sm" onclick={() => (editing = !editing)}>
        {editing ? 'Cancel edit' : 'Edit'}
      </button>
      <form method="POST" action="?/delete" use:enhance>
        <button
          type="submit"
          class="btn btn-danger btn-sm"
          onclick={(e) => { if (!confirm('Delete this recipe?')) e.preventDefault(); }}
        >Delete</button>
      </form>
    </div>
  </div>

  {#if form?.error}
    <div class="alert alert-error">{form.error}</div>
  {/if}
  {#if form?.success}
    <div class="alert alert-success">Recipe updated!</div>
  {/if}

  {#if editing}
    <!-- Edit form -->
    <div class="card">
      <div class="card-body">
        <form method="POST" action="?/update" use:enhance={() => {
          return async ({ update }) => { await update(); editing = false; };
        }}>
          <div class="form-group">
            <label for="title">Recipe name *</label>
            <input id="title" name="title" type="text" value={r.title} required />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label for="prep_time">Prep time (min)</label>
              <input id="prep_time" name="prep_time" type="number" value={r.prep_time ?? ''} />
            </div>
            <div class="form-group">
              <label for="servings">Servings</label>
              <input id="servings" name="servings" type="number" value={r.servings ?? ''} />
            </div>
          </div>
          <div class="form-group">
            <label for="description">Description</label>
            <textarea id="description" name="description" rows="2">{r.description ?? ''}</textarea>
          </div>
          <div class="form-group">
            <label for="ingredients">Ingredients (one per line)</label>
            <textarea id="ingredients" name="ingredients" rows="6">{r.ingredients.join('\n')}</textarea>
          </div>
          <div class="form-group">
            <label for="instructions">Instructions</label>
            <textarea id="instructions" name="instructions" rows="8">{r.instructions ?? ''}</textarea>
          </div>
          <button type="submit" class="btn btn-primary">Save changes</button>
        </form>
      </div>
    </div>
  {:else}
    <!-- View mode -->
    <div style="display:grid;grid-template-columns:1fr 2fr;gap:1rem;align-items:start">
      <!-- Meta -->
      <div class="card">
        <div class="card-body">
          {#if r.prep_time}
            <div class="mb-1"><span class="text-muted text-sm">Prep time</span><br /><strong>⏱ {r.prep_time} min</strong></div>
          {/if}
          {#if r.servings}
            <div class="mb-1"><span class="text-muted text-sm">Servings</span><br /><strong>👥 {r.servings}</strong></div>
          {/if}
          {#if r.description}
            <p class="text-sm" style="margin-top:0.5rem">{r.description}</p>
          {/if}

          {#if r.ingredients.length > 0}
            <div style="margin-top:1rem">
              <div class="font-semibold mb-1" style="font-size:0.85rem">Ingredients</div>
              <ul style="padding-left:1.2rem;font-size:0.875rem;line-height:2">
                {#each r.ingredients as ing}
                  <li>{ing}</li>
                {/each}
              </ul>
            </div>
          {/if}
        </div>
      </div>

      <!-- Instructions -->
      {#if r.instructions}
        <div class="card">
          <div class="card-header"><span class="card-title">Instructions</span></div>
          <div class="card-body" style="white-space:pre-wrap;font-size:0.9rem;line-height:1.7">
            {r.instructions}
          </div>
        </div>
      {:else}
        <div class="empty-state">No instructions added yet.</div>
      {/if}
    </div>
  {/if}
</div>
