---
name: catppuccin-design
description: Apply the Catppuccin design system (Mocha dark default, Latte light fallback) to any frontend code Vibe writes or reviews. Use this skill whenever the user asks to build, style, theme, or refactor a UI — websites, dashboards, components, landing pages, CLI HTML reports, or anything with CSS — and also whenever the user mentions "Catppuccin", "Mocha", "Latte", "design system", "style guide", "theme my app", "make this look nice", or pastes raw unstyled HTML/CSS/JSX. Generates production-grade CSS variables, Tailwind config, and reusable components instead of letting the model invent ad-hoc colors.
license: MIT
compatibility: any
user-invocable: true
allowed-tools:
  - read_file
  - write_file
  - search_replace
  - grep
  - bash
  - ask_user_question
---

# Catppuccin Design System

A reusable design system for all frontend work. Default flavor is **Mocha** (dark); **Latte** (light) is provided as an accessible alternative. The goal is consistent, polished UI without re-deriving color choices every project.

## When to trigger

Activate this skill on any of the following:

- User asks to build/style/theme any frontend (HTML, CSS, React, Vue, Svelte, Astro, etc.)
- User mentions Catppuccin, Mocha, Latte, Frappé, or Macchiato
- User asks for a "design system", "style guide", "theme", "color palette", or "make this look good"
- User pastes unstyled markup and asks for styling
- User is generating a dashboard, landing page, admin panel, marketing page, or component library

If you're unsure whether to apply it, apply it — the cost is low and the consistency gain is high.

## Slash command

When invoked as `/catppuccin-design`, prompt the user for:

1. **Target** — new project, existing file, or component snippet
2. **Stack** — vanilla CSS, Tailwind, CSS Modules, styled-components, or other
3. **Default flavor** — Mocha (dark) or Latte (light), or both with a toggle

Then generate the appropriate boilerplate using the rules below.

## Core rules — never deviate

### 1. Color foundation

- **Default flavor:** Mocha (dark). Always provide Latte (light) as a sibling theme.
- **Brand roles:**
  - Primary action → `Mauve`
  - Secondary → `Blue`
  - Accent → `Sapphire`
- **Semantic roles:**
  - Success → `Green`
  - Warning → `Yellow`
  - Error → `Red`
  - Info → `Sky`
- **Never invent hex codes.** Pull from `references/palette.md` (all 26 colors per flavor).

### 2. Surfaces and text hierarchy

| Layer            | Token       |
| ---------------- | ----------- |
| Page background  | `base`      |
| Sidebar / modal  | `mantle`    |
| Footer / depth   | `crust`     |
| Hover state      | `surface0`  |
| Active/pressed   | `surface1`  |
| Border           | `overlay1`  |
| Heading text     | `text`      |
| Body text        | `subtext1`  |
| Muted / caption  | `overlay0`  |

### 3. Visual language

- `border-radius`: **8px** for cards, buttons, inputs. **12px** for modals/sheets. **4px** for chips/badges.
- Shadows: derive from `crust` at low opacity (e.g. `0 4px 12px rgba(17, 17, 27, 0.4)` in Mocha). **Never pure black.**
- Gradients: linear `Mauve → Pink` for hero/feature surfaces only. Don't overuse.
- Fonts: **Inter** for UI, **JetBrains Mono** for code. Always include a system fallback stack.

### 4. Code output requirements

- All colors **must** be CSS variables on `:root` and `[data-theme="latte"]`. No inline hex codes in components.
- For Tailwind projects, write a `tailwind.config.js` `extend.colors` block mapping `ctp-mocha-*` and `ctp-latte-*` namespaces.
- Code blocks and syntax highlighting use official Catppuccin Prism/Shiki/Highlight.js themes (note in output, link to repo).
- High-contrast mode: swap primary action `Mauve` → `Lavender`, accent `Sapphire` → `Rosewater`, and bump body text to `Text` instead of `Subtext1`.

## Workflow

When the skill triggers (explicitly or implicitly):

1. **Read** `references/palette.md` for the full hex table. Don't memoize — re-read each time to avoid drift.
2. **Read** the relevant component template in `assets/` that matches the user's request (button, card, root.css, tailwind.config.js).
3. **Apply** the rules above to the user's code or generate fresh boilerplate.
4. **Verify** the output uses only token names, never raw hex, in component-level code.

## Deliverables checklist

For a fresh project setup, produce in this order:

- [ ] `styles/catppuccin.css` — `:root` variables for both flavors + theme toggle
- [ ] `tailwind.config.js` (if Tailwind) — extended color palette
- [ ] One example component (button or card) demonstrating tokens in use
- [ ] Brief README section explaining theme switching and high-contrast mode

For styling an existing file:

- [ ] Replace hex codes with CSS variable references
- [ ] Add `:root` block if missing
- [ ] Confirm border-radius, font, and shadow rules

## Reference files

- `references/palette.md` — All 26 hex codes for Mocha and Latte. **Always read before emitting colors.**
- `references/usage-patterns.md` — Common pitfalls, do/don't examples, accessibility notes.
- `assets/catppuccin.css` — Drop-in `:root` boilerplate.
- `assets/tailwind.config.js` — Tailwind extension snippet.
- `assets/button.html` — Reference button component.
- `assets/card.jsx` — Reference React card component.

## Quick links

- Palettes: https://github.com/catppuccin/palette
- Tailwind plugin: https://github.com/catppuccin/tailwind
- Syntax highlighting variants: https://github.com/catppuccin (Prism, Shiki, Highlight.js)
