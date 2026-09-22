# AVI - Typst Document Standards & Conventions

This document describes the conventions followed by the `*.typ` documents in this repository (UPIICSA, *Ambientes Virtuales Inmersivos*, 6NM69). Not every file uses every rule; they describe the broadly established patterns. All AI assistants and contributors should follow them when creating or editing documents.

## 1. Folder Layout
- **Grouped by school period** at the repo root (`2026-1/`, `2026-2/`, …). Each period holds its documents plus any non-Typst work for that period (e.g. the Unity project `2026-2/AVI/`, the HTML presentation `2026-2/proyecto-final/`).
- **One folder per deliverable** inside the period. Team work: `<periodo>/docs/<deliverable>/`. Individual work: `<periodo>/individuales/<apellidosnombre>/<reto N>/<reto N-M>/`.
- **Self-contained folders**: each folder holds one main `.typ`, its own copy of `portada-template.typ`, and a `media/` directory. Nothing is imported from outside the folder.
- **Assets in `media/`**: logos at `media/logos/IPN_Logo.svg` and `media/logos/UPIICSA_Logo.svg`; screenshots, QR codes (`.svg`) and the bibliography (`referencias.bib`) directly in `media/`.
- **Naming**: folders use kebab-case or camelCase (`exposicion-frameVR`, `proyectoAVI`). A reto's main file is the reto name without spaces (`retoii-1.typ`); other documents get a short descriptive name (`doc-lentes.typ`, `texto-expo.typ`).
- **PDFs are build output** and are git-ignored (`*.pdf`); never commit them.

## 2. Main File Structure
Every main file follows this order:
1. **Imports**: `#import "portada-template.typ": portada` (plus any packages actually used).
2. **Raw text**: `#show raw: set text(font: "JetBrainsMono NFM", weight: "medium", size: 1em)`.
3. **Codly** (when the document contains code): `@preview/codly:1.3.0`, `#show: codly-init.with()`, a `languages:` map with name, icon and brand `rgb` color per language, and `number-format: n => str(n)`.
4. **Global text**: `#set text(font: "ITC Avant Garde Gothic", lang: "es", weight: "semibold")`.
5. **Headings** (optional): `#show heading: set text(font: "Fraunces 72pt", weight: "black")`.
6. **Page**: `paper: "us-letter"`, `margin: (left: 3cm, top: 2.5cm, right: 2.5cm, bottom: 2.5cm)`, `numbering: "1"`.
7. **Members**: `#let integrantes = (...)`, an array of `"Apellidos Nombre - Boleta"` strings (single entry for individual work).
8. **Cover**: `#portada(...)` followed by `#pagebreak()`.
9. **Body settings**: `#set par(justify: true, leading: 1.4em)`, `#set heading(numbering: "1.")`, `#set list(indent: 1.5em)`.
10. **Title**: `#title("Reto X.Y Nombre")` or `#title[...]`.
11. **Content**.
12. **Bibliography** (when citing): `#bibliography("media/referencias.bib", style: "apa")` as the last line.

## 3. Cover (`portada-template.typ`)
- **Signature**: positional only; 7 labels (`titulo_*`) followed by 7 values, snake_case Spanish names.
- **Canonical call**: labels `UNIDAD DE APRENDIZAJE`, `LÍNEA CURRICULAR`, `EQUIPO`, `PERIODO`, `INTEGRANTES`, `SECUENCIA`, `PROFESORA`; values: course name, `Simulación de Ambientes Virtuales`, `Equipo 5`, `2026 - 2`, `integrantes`, `6NM69`, `Bustamante Tranquilino Rocío`.
- **Slots are repurposed**: parameter names do not match their content (e.g. `titulo_secuencia` receives "PERIODO"). What matters is that each label and its value occupy matching positions. Prefer annotating each argument with a `// titulo_x` comment.
- **Layout**: IPN and UPIICSA logos in a two-column grid at 3.5cm; institution names semibold at 18pt / 17pt; two-column field grid with each label at 11pt `stretch: 75%` above its value at 12pt semibold.
- **Members list**: arrays render as a list; the preferred form is sorted and left-aligned: `align(left, list(..alumnos.sorted()))`.
- **Comments**: section banners as `// === Section ===` and `// --- Row N ---`.

## 4. Content Conventions
- **Language**: Spanish for prose, identifiers, and comments.
- **Headings**: `=`, `==`, `===`. Individual retos often phrase headings as the assignment's question (`= ¿Qué papel juega…?`) and close with `== Conclusión:`.
- **Lists**: `- *Término:* explicación.` Team documents put a blank line between items; retos may keep them tight. Sub-items are indented two spaces.
- **Emphasis**: `*bold*` for key terms and figures (`*5 y 20 USD*`); backticks for technical terms or acronyms (`NPCs`).
- **Links**: `#link("url")[label]`, where the label is the full URL or a short action ("Abrir …", `*Drive*`). Long links break onto their own indented line inside `[...]`.
- **Images**: screenshots as `#figure(image(..., width: 100%), caption: [...])`; QR codes as `#align(center)[#image(..., width: 4cm)]`.
- **Citations**: `@key` before the final period; BibTeX keys follow `autorAñoPalabra` (`bhowmik2024virtual`).
- **Page breaks**: manual `#pagebreak()` to pace long documents.
- **Math**: inline only, mainly for units (`$m^2$`).

## 5. Formatting
- **Indentation**: 2 spaces; trailing commas on multi-line argument lists (typstyle style).
- **Colors**: only via `rgb("#hex")`.
- **Themed variant** (see `retoii-1`): page fill `#F7F6F1`, text `#2A2A2A`, headings `#E67D22`; tables colour their header row with `fill: (x, y) => if y == 0 { … }`.
- **Comments**: sparse. English section banners in templates, Spanish inline notes.

## 6. Anti-patterns (do not repeat)
These exist in current files but should not be copied into new ones:
- **Unused imports**: do not import `@preview/cetz` unless drawing with it, and do not set up codly in documents without code blocks.
- **Diverging template copies**: when copying `portada-template.typ` into a new folder, copy the latest version (currently `2026-2/docs/proyectoAVI/portada-template.typ`), not an older one.
- **Inconsistent course name**: use the full name "Ambientes Virtuales Inmersivos" on the cover, not "Ambientes Virtuales".
- **Mixing heading styles**: pick one heading font per document and do not mix Fraunces and body-font headings across related deliverables.
- **Malformed headings**: never double the marker (`= = Título`, seen in `2026-2/docs/Lentes-virtuales/doc-lentes.typ`).
- **Ad-hoc titles**: use `#title(...)` rather than a hand-styled `#align(center)[#text(22pt, ...)]` block.
- **Unannotated positional cover calls**: do not pass the 14 `portada` arguments without `// titulo_x` comments; the repurposed slots make them easy to misorder.
