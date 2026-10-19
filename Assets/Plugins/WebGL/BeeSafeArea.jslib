mergeInto(LibraryManager.library, {
  // Returns a device safe-area inset normalized to the viewport (0..1).
  //   side: 0 = top, 1 = bottom, 2 = left, 3 = right
  // Uses CSS env(safe-area-inset-*) (the app template opts into viewport-fit=cover),
  // which matches what the Toss WebView reports. Debug override:
  //   window.__beeForceSafeArea = {top,bottom,left,right}  (normalized)
  __BeeSafeAreaInset: function (side) {
    try {
      var forced = window.__beeForceSafeArea;
      if (forced) {
        switch (side) {
          case 0: return forced.top || 0;
          case 1: return forced.bottom || 0;
          case 2: return forced.left || 0;
          case 3: return forced.right || 0;
        }
      }
      var d = document.createElement('div');
      d.style.cssText =
        'position:fixed;left:0;top:0;width:0;height:0;visibility:hidden;pointer-events:none;' +
        'padding-top:env(safe-area-inset-top,0px);' +
        'padding-bottom:env(safe-area-inset-bottom,0px);' +
        'padding-left:env(safe-area-inset-left,0px);' +
        'padding-right:env(safe-area-inset-right,0px);';
      document.body.appendChild(d);
      var cs = window.getComputedStyle(d);
      var t = parseFloat(cs.paddingTop) || 0;
      var b = parseFloat(cs.paddingBottom) || 0;
      var l = parseFloat(cs.paddingLeft) || 0;
      var r = parseFloat(cs.paddingRight) || 0;
      document.body.removeChild(d);
      var w = window.innerWidth || 1, h = window.innerHeight || 1;
      switch (side) {
        case 0: return t / h;
        case 1: return b / h;
        case 2: return l / w;
        case 3: return r / w;
      }
    } catch (e) { }
    return 0;
  }
});
