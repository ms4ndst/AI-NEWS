# Catppuccin Usage Patterns

## Do / Don't

### Surfaces

- ✅ Stack surfaces by depth: page = `base`, raised card = `mantle`, deepest accent = `crust`. Reverse for Latte if it looks washed out.
- ❌ Don't put `crust` on a `base` background — they're nearly identical in Mocha. The contrast jump is the whole point of three tiers.

### Text

- ✅ Headings on `text`, body on `subtext1`, captions on `overlay0`. This creates clear hierarchy without changing font weight everywhere.
- ❌ Never put `overlay0` text on a `surface0` background — fails WCAG AA on Mocha.

### Interactive states

- Default → no surface color (transparent on `base`)
- Hover → `surface0`
- Active / pressed → `surface1`
- Disabled → `surface2` background, `overlay0` text
- Focus ring → 2px solid `mauve` (or `lavender` in high-contrast)

### Semantic colors

- Use the semantic role color (red/yellow/green/sky) as both border and 10–15% opacity background for alert components. Body text stays on `text` for readability — don't color the message text itself unless it's a single-line toast.

## Accessibility

- Mocha + `text`/`subtext1` on `base`/`mantle` clears WCAG AA at body sizes. Verify before using `subtext0` or `overlay2` for any reading text.
- Provide a `prefers-color-scheme` media query that selects Latte when the OS is in light mode, and a manual toggle that overrides it via `data-theme`.
- High-contrast mode: swap `mauve` → `lavender`, `sapphire` → `rosewater`, and bump all body text up one tier (`subtext1` → `text`, `overlay0` → `subtext1`).

## Common mistakes

1. **Using `red` for both error and destructive primary buttons.** Pick one. Error states use red border + tinted background; destructive actions use red as a full button background only when the action is genuinely irreversible.
2. **Gradients on every section.** Mauve→Pink is for a single hero element per page. Beyond that it becomes noisy.
3. **Mixing Catppuccin colors with non-Catppuccin grays.** If you need a neutral, it's one of `surface*` / `overlay*` / `subtext*`. Always.
4. **Forgetting the Latte theme.** Always emit both `:root` and `[data-theme="latte"]` blocks, even if the user only asked for dark.
5. **Border-radius drift.** 8px is the default. Don't sprinkle 6px, 10px, 16px around — pick from {4, 8, 12} and stick to the layer rule.

## Tailwind-specific

- The official `@catppuccin/tailwindcss` plugin handles namespacing automatically. Prefer the plugin over a hand-rolled `extend` block when possible, and only roll your own if the project pins an older Tailwind version.
- Class names look like `bg-ctp-base`, `text-ctp-subtext1`, `border-ctp-overlay1`. Document this in the project README — devs unfamiliar with Catppuccin will hunt for `bg-gray-900` and get confused.
