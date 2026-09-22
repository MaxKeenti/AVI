/* Scale the 1600×900 canvas to fit the viewport, preserving 16:9.
   Loaded by every slide after the .scaler element. */
(function () {
  var scaler = document.querySelector('.scaler');
  if (!scaler) return;
  function fit() {
    var s = Math.min(window.innerWidth / 1600, window.innerHeight / 900);
    scaler.style.transform = 'scale(' + s + ')';
    scaler.style.width = (1600 * s) + 'px';
    scaler.style.height = (900 * s) + 'px';
  }
  window.addEventListener('resize', fit);
  fit();
})();
