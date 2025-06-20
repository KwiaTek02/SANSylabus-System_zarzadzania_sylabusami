window.exportPdfFromDiv = function (elementId, filename) {
    // Pobiera element HTML na podstawie jego ID
    const element = document.getElementById(elementId);

    // Ustawienia dla generowania PDF
    const opt = {
        margin: [1, 0, 1, 0],  // marginesy w calach: góra, lewa, dół, prawa
        filename: filename,    // nazwa pliku wynikowego
        image: { type: 'jpeg', quality: 0.98 },  // jakość obrazów w PDF
        html2canvas: {
            scale: 2,           // zwiększa rozdzielczość renderowania
            useCORS: true,      // umożliwia ładowanie zasobów z innych domen (jeśli są dostępne)
            scrollY: 0,         // ignoruje pozycję przewinięcia strony
            logging: false      // wyłącza logowanie w konsoli
        },
        jsPDF: {
            unit: 'in',         // jednostki w calach
            format: 'a4',       // format strony PDF
            orientation: 'portrait' // orientacja pionowa
        },
        pagebreak: {
            mode: ['css', 'legacy'], // sposób dzielenia stron
            avoid: [                 // unikanie dzielenia konkretnych elementów
                'img', 'table', 'h1', 'h2', 'h3',
                '.card', '.section', '.no-break'
            ]
        }
    };

    // Zbiera elementy, które powinny zostać ukryte podczas generowania PDF
    const toHide = [
        ...document.querySelectorAll(
            '.btn-action, .nav, .no-print, .year-tab-btn.active, .year-tab-btn:hover, .year-tab-btn, .year-tab-nav'
        )
    ];

    // Zapamiętuje oryginalne wartości stylu `display` tych elementów
    const originalDisplay = toHide.map(el => el.style.display);

    // Ukrywa te elementy, aby nie były widoczne w PDF
    toHide.forEach(el => el.style.display = 'none');

    // Opóźnia wykonanie eksportu o 800 ms, aby ukrywanie zdążyło się zastosować
    setTimeout(() => {
        // Tworzy PDF z wybranego elementu i zapisuje go
        html2pdf().set(opt).from(element).save().then(() => {
            // Po zapisaniu PDF przywraca oryginalny wygląd ukrytych elementów
            toHide.forEach((el, i) => el.style.display = originalDisplay[i]);
        });
    }, 800);
}
