/**
 * Kullanıcı Yönetimi DataTables Modülü
 */
const KullaniciTableModule = (function () {
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
                    console.error("AJAX Hatası:", thrown);
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
            // 1. Sütun: İşlemler
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
                                    data-id="${row.id}" data-name="${row.ad || row.Ad} ${row.soyad || row.Soyad}">Sil</button>
                        </div>
                    `;
                },
                "orderable": false,
                "searchable": false
            },
            // 2. Sütun: Ad Soyad (Solunda küçük yuvarlak profil fotoğrafı / harf logosu ile birlikte)
            { 
                "data": null,
                "render": function(data, type, row) { 
                    const ad = row.ad || row.Ad || "";
                    const soyad = row.soyad || row.Soyad || "";
                    const imgPath = row.profileImagePath || row.ProfileImagePath;
                    const imgSrc = imgPath ? imgPath : `https://ui-avatars.com/api/?name=${ad}+${soyad}&background=random&color=fff`;

                    return `
                        <div class="d-flex align-items-center gap-2">
                            <img src="${imgSrc}" class="rounded-circle shadow-sm" style="width: 36px; height: 36px; object-fit: cover;" alt="Profil">
                            <span class="fw-semibold">${ad} ${soyad}</span>
                        </div>
                    `; 
                }
            },
            // 3. Sütun: Email
            { 
                "data": "email",
                "render": function(data, type, row) { 
                    return `<span class="text-muted">${data || row.Email || ""}</span>`; 
                }
            },
            // 4. Sütun: Departman
            { 
                "data": "departmanAd",
                "render": function(data, type, row) { 
                    const dept = data || row.DepartmanAd || "Belirtilmemiş";
                    return `<span class="dept-pill">${dept}</span>`; 
                }
            },
            // 5. Sütun: Kayıt Tarihi
            { 
                "data": "kayitTarihi",
                "render": function(data, type, row) {
                    return data || row.KayitTarihi || "";
                }
            },
            // 6. Sütun: Durum (Aktif / Pasif)
            { 
                "data": "isActive",
                "className": "text-end pe-4",
                "render": function (data, type, row) {
                    const status = (data !== undefined) ? data : row.IsActive;
                    return status 
                        ? '<span class="badge bg-success bg-opacity-10 text-success border border-success">Aktif</span>' 
                        : '<span class="badge bg-danger bg-opacity-10 text-danger border border-danger">Pasif</span>';
                }
            }
        ];
    };

    const _bindEvents = function (config) {
        _$table.find('tbody').on('click', '.btn-detay', function () {
            const rowData = JSON.parse(decodeURIComponent($(this).attr('data-row')));
            
            // Verileri Hazırlama
            const imgPath = rowData.profileImagePath || rowData.ProfileImagePath;
            const ad = rowData.ad || rowData.Ad || "";
            const soyad = rowData.soyad || rowData.Soyad || "";
            const fallbackSrc = imgPath ? imgPath : `https://ui-avatars.com/api/?name=${ad}+${soyad}&background=random&color=fff`;
            
            // SOL TARAF (Fotoğraf ve Özet)
            $('#modalProfileImage').attr('src', fallbackSrc);
            $('#modalAdSoyadTitle').text(ad + " " + soyad);
            $('#modalDepartmanBadge').text(rowData.departmanAd || rowData.DepartmanAd || "");

            // SAĞ TARAF (Detaylı Liste)
            $('#modalAdSoyad').text(ad + " " + soyad);
            $('#modalEmail').text(rowData.email || rowData.Email || "");
            $('#modalDepartman').text(rowData.departmanAd || rowData.DepartmanAd || "");
            $('#modalKayitTarihi').text(rowData.kayitTarihi || rowData.KayitTarihi || "");
            
            const isActive = (rowData.isActive !== undefined) ? rowData.isActive : rowData.IsActive;
            $('#modalDurum').text(isActive ? "Aktif" : "Pasif");
            
            // Modalı Göster
            const detayModal = new bootstrap.Modal(document.querySelector(config.modalSelector));
            detayModal.show();
        });

        const deleteModal = document.querySelector(config.deleteModalSelector);
        if (deleteModal) {
            deleteModal.addEventListener('show.bs.modal', event => {
                const button = event.relatedTarget;
                document.getElementById('deleteModalUserName').textContent = button.getAttribute('data-name');
                document.getElementById('deleteModalForm').action = '/Home/Delete/' + button.getAttribute('data-id');
            });
        }
    };