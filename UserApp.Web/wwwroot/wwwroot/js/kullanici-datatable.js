/**
 * Kullanıcı Yönetimi DataTables Modülü
 * Sorumluluk: Tablonun çizilmesi, AJAX istekleri ve Modal (Pop-up) yönetimini soyutlar.
 */
const KullaniciTableModule = (function () {
    // Özel (Private) Değişkenler
    let _dataTableInstance = null;
    let _$table = null;

    const _initializeTable = function (config) {
        _$table = $(config.tableSelector);
        

        const ajaxEndpoint = _$table.data('url');

        if (!ajaxEndpoint) {
            console.error("HATA: Tabloya data-url attribute'u eklenmemiş!");
            return;
        }

        _dataTableInstance = _$table.DataTable({
            "ajax": {
                "url": ajaxEndpoint,
                "type": "GET",
                "datatype": "json",
                "error": function (xhr, error, thrown) {
                    console.error("AJAX Hatası: Endpoint'e ulaşılamadı. Controller adını kontrol et.", thrown);
                }
            },
            "dom": "<'row mb-3'<'col-sm-12 col-md-6'B><'col-sm-12 col-md-6'f>>" +
                   "<'row'<'col-sm-12'tr>>" +
                   "<'row mt-3'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            "buttons": [
                { extend: 'excelHtml5', text: 'Excel İndir', className: 'btn btn-admin-primary btn-sm' },
                { extend: 'colvis', text: 'Sütunları Yönet', className: 'btn btn-admin-outline btn-sm' }
            ],
            "columns": _getColumnsConfig(),
            "language": {
                "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json"
            }
        });
    };

    const _getColumnsConfig = function () {
        return [
            { 
                "data": null,
                "render": function (data, type, row) {
                    const rowData = encodeURIComponent(JSON.stringify(row));
                    return `
                        <div class="d-flex gap-1 ps-2">
                            <a href="/Home/Edit/${row.id}" class="btn btn-admin-outline btn-sm">Düzenle</a>
                            <button type="button" class="btn btn-admin-outline btn-sm btn-detay" data-row="${rowData}">Detay</button>
                            <button type="button" class="btn btn-admin-danger-soft btn-sm" 
                                    data-bs-toggle="modal" data-bs-target="#deleteModal" 
                                    data-id="${row.id}" data-name="${row.ad} ${row.soyad}">Sil</button>
                        </div>
                    `;
                },
                "orderable": false,
                "searchable": false
            },
            { 
                "data": null,
                "render": function(data, type, row) { return `<span class="fw-semibold">${row.ad} ${row.soyad}</span>`; }
            },
            { 
                "data": "email",
                "render": function(data) { return `<span class="text-muted">${data}</span>`; }
            },
            { 
                "data": "departmanAd",
                "render": function(data) { return `<span class="dept-pill">${data}</span>`; }
            },
            { "data": "kayitTarihi" },
            { 
                "data": "isActive",
                "className": "text-end pe-4",
                "render": function (data) {
                    return data 
                        ? '<span class="badge bg-success bg-opacity-10 text-success border border-success">Aktif</span>' 
                        : '<span class="badge bg-danger bg-opacity-10 text-danger border border-danger">Pasif</span>';
                }
            }
        ];
    };

    const _bindEvents = function (config) {
        // Detay Butonu Olayı 
        _$table.find('tbody').on('click', '.btn-detay', function () {
            const rowData = JSON.parse(decodeURIComponent($(this).attr('data-row')));
            
            $('#modalAdSoyad').text(rowData.ad + " " + rowData.soyad);
            $('#modalEmail').text(rowData.email);
            $('#modalDepartman').text(rowData.departmanAd);
            $('#modalKayitTarihi').text(rowData.kayitTarihi);
            $('#modalDurum').text(rowData.isActive ? "Aktif" : "Pasif");
            
            const detayModal = new bootstrap.Modal(document.querySelector(config.modalSelector));
            detayModal.show();
        });

        // Silme İşlemi 
        const deleteModal = document.querySelector(config.deleteModalSelector);
        if (deleteModal) {
            deleteModal.addEventListener('show.bs.modal', event => {
                const button = event.relatedTarget;
                document.getElementById('deleteModalUserName').textContent = button.getAttribute('data-name');
                document.getElementById('deleteModalForm').action = '/Home/Delete/' + button.getAttribute('data-id');
            });
        }
    };

    return {
        init: function (config) {
            _initializeTable(config);
            _bindEvents(config);
        }
    };
})();