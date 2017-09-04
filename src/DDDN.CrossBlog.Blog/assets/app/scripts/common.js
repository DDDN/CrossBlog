function HighlightActiveLink() {
   for (var i = 0; i < document.links.length; i++) {
      if (document.links[i].href == document.URL) {
         document.links[i].className += ' active';
      }
   }
};