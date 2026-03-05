<script lang="ts">
  import type { ActionData } from './$types';
  let { form }: { form: ActionData } = $props();
</script>

<svelte:head><title>New recipe – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">New recipe</div>
    </div>
    <a href="/recipes" class="btn btn-ghost btn-sm">← Back</a>
  </div>

  {#if form?.error}
    <div class="alert alert-error">{form.error}</div>
  {/if}

  <div class="card">
    <div class="card-body">
      <form method="POST">
        <div class="form-group">
          <label for="title">Recipe name *</label>
          <input id="title" name="title" type="text" placeholder="e.g. Spaghetti Bolognese" required />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="prep_time">Prep time (minutes)</label>
            <input id="prep_time" name="prep_time" type="number" min="1" placeholder="30" />
          </div>
          <div class="form-group">
            <label for="servings">Servings</label>
            <input id="servings" name="servings" type="number" min="1" placeholder="4" />
          </div>
        </div>

        <div class="form-group">
          <label for="description">Short description</label>
          <textarea id="description" name="description" rows="2" placeholder="A quick description of the dish…"></textarea>
        </div>

        <div class="form-group">
          <label for="ingredients">Ingredients (one per line)</label>
          <textarea id="ingredients" name="ingredients" rows="6" placeholder="200g pasta&#10;1 onion&#10;2 cloves garlic&#10;…"></textarea>
        </div>

        <div class="form-group">
          <label for="instructions">Instructions</label>
          <textarea id="instructions" name="instructions" rows="8" placeholder="Step by step instructions…"></textarea>
        </div>

        <div class="flex gap-1">
          <button type="submit" class="btn btn-primary">Save recipe</button>
          <a href="/recipes" class="btn btn-ghost">Cancel</a>
        </div>
      </form>
    </div>
  </div>
</div>
