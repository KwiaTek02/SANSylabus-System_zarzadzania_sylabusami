//window.exportPdfFromDiv = async function (elementId, filename) {
    //const { jsPDF } = window.jspdf;
    //const input = document.getElementById(elementId);

    // Znajdź i ukryj wszystkie elementy, które nie powinny być w PDF
    //const toHide = [
       // ...document.querySelectorAll('.btn-action'),
        //...document.querySelectorAll('.year-tab-nav'),
//        ...document.querySelectorAll('.nav'),
//        ...document.querySelectorAll('.nav-pills'),
//        ...document.querySelectorAll('.no-print')
//    ];

    // Zapisz oryginalne style i ukryj
 //   const originalStyles = [];
 //   toHide.forEach(el => {
  //      originalStyles.push({ el, display: el.style.display });
 //       el.style.display = 'none';
 //   });

    // Czekaj aż przeglądarka przerysuje DOM
 //   await new Promise(resolve => setTimeout(resolve, 200));

 //   await html2canvas(input, {
 //       scale: 2,
 //       useCORS: true
 //   }).then(canvas => {
 //       const imgData = canvas.toDataURL('image/png');
//        const pdf = new jsPDF('p', 'mm', 'a4');
//
//        const pageWidth = pdf.internal.pageSize.getWidth();
//        const pageHeight = pdf.internal.pageSize.getHeight();
//
 //       const imgProps = pdf.getImageProperties(imgData);
 //       const pdfWidth = pageWidth;
//        const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

//        let position = 0;
//            pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
  //      } else {
  //          let heightLeft = pdfHeight;
  //          while (heightLeft > 0) {
   //             pdf.addImage(imgData, 'PNG', 0, position, pdfWidth, pdfHeight);
  //              heightLeft -= pageHeight;
  //              if (heightLeft > 0) {
  //                  pdf.addPage();
  //                  position = -heightLeft + 10;
  //              }
  //          }
//        }

     //   pdf.save(filename);
   // });

    // Przywróć oryginalne style
  //  originalStyles.forEach(({ el, display }) => {
  //      el.style.display = display;
  //  });
//};

window.exportPdfFromDiv = function (elementId, filename) {
    const element = document.getElementById(elementId);

    const opt = {
        margin: [0.5, 0.5, 0.5, 0.5],  // top, left, bottom, right in inches
        filename: filename,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2, useCORS: true, scrollY: 0, logging: false },
        jsPDF: { unit: 'in', format: 'a4', orientation: 'portrait' },
        pagebreak: { mode: ['avoid-all', 'css', 'legacy'] }
    };

    // Ukryj przyciski i zakładki
    const toHide = [...document.querySelectorAll('.btn-action, .nav, .no-print, .year-tab-btn.active, .year-tab-btn:hover, .year-tab-btn, .year-tab-nav')];
    const originalDisplay = toHide.map(el => el.style.display);
    toHide.forEach(el => el.style.display = 'none');

    setTimeout(() => {
        html2pdf().set(opt).from(element).save().then(() => {
            // Przywróć widoczność
            toHide.forEach((el, i) => el.style.display = originalDisplay[i]);
        });
    }, 800);
}
