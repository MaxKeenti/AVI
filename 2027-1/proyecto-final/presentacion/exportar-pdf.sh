#!/usr/bin/env bash
# Genera consultorio-seguro-presentacion.pdf a partir de imprimir.html, con
# Chrome (o Chromium, Edge, Brave) sin interfaz. El orden sale de slides.js.
#
# Uso:   ./exportar-pdf.sh [salida.pdf]
# Otro navegador:   CHROME=/ruta/al/navegador ./exportar-pdf.sh
set -euo pipefail

dir="$(cd "$(dirname "$0")" && pwd)"
salida="${1:-$dir/consultorio-seguro-presentacion.pdf}"

buscar_chrome() {
  local c
  for c in \
    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome" \
    "/Applications/Chromium.app/Contents/MacOS/Chromium" \
    "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge" \
    "/Applications/Brave Browser.app/Contents/MacOS/Brave Browser"; do
    [[ -x "$c" ]] && { echo "$c"; return; }
  done
  for c in google-chrome google-chrome-stable chromium chromium-browser microsoft-edge brave-browser; do
    command -v "$c" >/dev/null 2>&1 && { command -v "$c"; return; }
  done
  return 1
}

chrome="${CHROME:-$(buscar_chrome || true)}"
if [[ -z "$chrome" ]]; then
  echo "No se encontró Chrome ni Chromium. Instálalo o indica la ruta con CHROME=..." >&2
  exit 1
fi

# Ruta como URL file:// (los espacios del repositorio van codificados).
url="file://$(printf '%s' "$dir/imprimir.html" | sed 's/ /%20/g')"

"$chrome" --headless=new --disable-gpu --hide-scrollbars \
  --no-pdf-header-footer --virtual-time-budget=15000 \
  --print-to-pdf="$salida" "$url" >/dev/null 2>&1

if [[ ! -s "$salida" ]]; then
  echo "Chrome no generó el PDF." >&2
  exit 1
fi
echo "PDF listo: $salida"
