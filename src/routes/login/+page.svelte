<script lang="ts">
  import type { ActionData } from './$types';
  let { form }: { form: ActionData } = $props();
</script>

<svelte:head><title>Login – HomePlanner</title></svelte:head>

<div class="login-wrap">
  <div class="login-card">
    <div class="login-title">🏠 HomePlanner</div>
    <div class="login-subtitle">Sign in to your household</div>

    {#if form?.error}
      <div class="alert alert-error">{form.error}</div>
    {/if}

    <form method="POST">
      <div class="form-group">
        <label for="username">Username</label>
        <input
          id="username"
          name="username"
          type="text"
          autocomplete="username"
          required
          autofocus
        />
      </div>
      <div class="form-group">
        <label for="password">Password</label>
        <input
          id="password"
          name="password"
          type="password"
          autocomplete="current-password"
          required
        />
      </div>
      <button type="submit" class="btn btn-primary w-full" style="margin-top:0.5rem">
        Sign in
      </button>
    </form>
  </div>
</div>
