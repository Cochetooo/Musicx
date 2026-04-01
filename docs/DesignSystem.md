# Musicx Design System (Cross-platform)

This document defines reusable visual tokens that can be applied to Web, Desktop, and future Mobile clients.

## Core principles

- Strong contrast first (readability before decorative style).
- Limited, coherent accent palette.
- Transparent surfaces for depth without reducing legibility.
- Shared semantic tokens (`Rating.*`) for consistent meaning.

## Rating colors

Defined in `Musicx.Application.Shared/Styling/UiColorPalette.cs`:

- Very low: `#8A1C3A`
- Low: `#C4492D`
- Mid: `#B78621`
- Good: `#2D9B63`
- Great: `#178D8D`
- Excellent: `#2D6ACB`
- Masterpiece: `#7A3FC8`

Neutral helpers:

- Empty / null rating: `#6B728066`
- Donut track: `#CBD5E166`

## Component guidance

- Use gradients with alpha (0.58–0.94) for rating chips.
- Keep borders close to base hue to preserve semantic color while improving contrast.
- Prefer a single accent hue per component state (avoid rainbow stacks in one element).