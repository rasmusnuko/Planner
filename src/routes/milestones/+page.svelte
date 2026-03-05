<script lang="ts">
  import { enhance } from '$app/forms';
  import type { PageData, ActionData } from './$types';

  let { data, form }: { data: PageData; form: ActionData } = $props();

  const today = new Date().toISOString().slice(0, 10);

  const suggestedEmojis = ['⭐', '🎉', '🏆', '❤️', '🌟', '🎂', '🚀', '🌈', '🦷', '👣', '🎓', '🏠', '✈️', '🎵'];
  let pickedEmoji = $state('⭐');
</script>

<svelte:head><title>Milestones – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">Milestones</div>
      <div class="page-subtitle">Family moments & achievements</div>
    </div>
  </div>

  {#if form?.error}
    <div class="alert alert-error">{form.error}</div>
  {/if}

  <!-- Add milestone form -->
  <div class="card mb-2">
    <div class="card-header"><span class="card-title">Add milestone</span></div>
    <div class="card-body">
      <form method="POST" action="?/add" use:enhance>
        <input type="hidden" name="emoji" value={pickedEmoji} />
        <div class="form-row" style="flex-wrap:wrap">
          <div class="form-group" style="flex:2;min-width:160px">
            <label for="ms-title">Title *</label>
            <input id="ms-title" name="title" type="text" placeholder="e.g. First steps!" required />
          </div>
          <div class="form-group" style="min-width:140px">
            <label for="ms-date">Date *</label>
            <input id="ms-date" name="date" type="date" value={today} required />
          </div>
        </div>

        <!-- Emoji picker -->
        <div class="form-group">
          <label>Emoji</label>
          <div class="flex gap-1" style="flex-wrap:wrap">
            {#each suggestedEmojis as emoji}
              <button
                type="button"
                style="background:{pickedEmoji === emoji ? 'var(--primary-light)' : 'transparent'};border:1px solid {pickedEmoji === emoji ? 'var(--primary)' : 'var(--border)'};border-radius:6px;padding:0.25rem 0.4rem;font-size:1.2rem;cursor:pointer;line-height:1"
                onclick={() => (pickedEmoji = emoji)}
              >{emoji}</button>
            {/each}
          </div>
        </div>

        <div class="form-group">
          <label for="ms-desc">Description (optional)</label>
          <textarea id="ms-desc" name="description" rows="2" placeholder="A little story about this moment…"></textarea>
        </div>
        <button type="submit" class="btn btn-primary btn-sm">Save milestone</button>
      </form>
    </div>
  </div>

  <!-- Timeline -->
  {#if data.milestones.length === 0}
    <div class="empty-state">No milestones yet. Record your first one above!</div>
  {:else}
    <div class="milestones-timeline">
      {#each data.milestones as m}
        <div class="milestone-item">
          <div class="milestone-emoji">{m.emoji}</div>
          <div class="milestone-content">
            <div class="milestone-date">{m.date}{#if m.creator_name} · <span style="color:{m.creator_color}">{m.creator_name}</span>{/if}</div>
            <div class="milestone-title">{m.title}</div>
            {#if m.description}
              <div class="milestone-desc">{m.description}</div>
            {/if}
          </div>
          <form method="POST" action="?/delete" use:enhance style="align-self:flex-start;margin-top:0.25rem">
            <input type="hidden" name="id" value={m.id} />
            <button type="submit" class="btn btn-danger btn-icon btn-sm" title="Delete">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/></svg>
            </button>
          </form>
        </div>
      {/each}
    </div>
  {/if}
</div>
