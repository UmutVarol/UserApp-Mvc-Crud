/**
 * Kullanıcı Yönetimi DataTables Modülü
 */
const KullaniciTableModule = (function () {
    let _dataTableInstance = null;
    let _$table = null;

    const _escapeHtml = function (value) {
        if (value === null || value === undefined) return '';
        return String(value)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    };

    const _resolveImagePath = function (rawPath, ad, soyad) {
        if (rawPath && rawPath.trim() !== "" && rawPath !== "undefined" && rawPath !== "null") {
            let cleanPath = rawPath.trim().replace(/\\/g, '/');

            if (cleanPath.startsWith('~')) {
                cleanPath = cleanPath.substring(1);
            }

            const wwwrootIndex = cleanPath.indexOf('wwwroot');
            if (wwwrootIndex !== -1) {
                cleanPath = cleanPath.substring(wwwrootIndex + 7);
            }

            if (!cleanPath.startsWith('http') && !cleanPath.startsWith('/')) {
                cleanPath = '/' + cleanPath;
            }

            return cleanPath;
        }

        const safeAd = encodeURIComponent(ad || 'K');
        const safeSoyad = encodeURIComponent(soyad || 'U');
        return `https://ui-avatars.com/api/?name=${safeAd}+${safeSoyad}&background=random&color=fff`;
    };

    /// YENİ: DepartmanYoneticisi'nin "Aktif/Pasif Yap" butonuna bastığında
    /// dinamik bir form oluşturup POST eder — DataTables satırları JS
    /// tarafından üretildiği için statik bir <form> yazamıyoruz, global
    /// antiforgery token'ı sayfadaki gizli formdan alıp bu dinamik forma taşıyoruz.
    const _submitStatusToggle = function (id, yeniDurum) {
        const tokenInput = document.querySelector('#globalAntiForgeryForm input[name="__RequestVerificationToken"]');
        if (!tokenInput) {
            console.error("HATA: Antiforgery token bulunamadı.");
            return;
        }

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Home/ToggleStatus';
        form.style.display = 'none';

        const tokenClone = document.createElement('input');
        tokenClone.type = 'hidden';
        tokenClone.name = '__RequestVerificationToken';
        tokenClone.value = tokenInput.value;

        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.name = 'id';
        idInput.value = id;

        const activeInput = document.createElement('input');
        activeInput.type = 'hidden';
        activeInput.name = 'isActive';
        activeInput.value = yeniDurum;

        form.appendChild(tokenClone);
        form.appendChild(idInput);
        form.appendChild(activeInput);
        document.body.appendChild(form);
        form.submit();
    };

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
            "columns": _getColumnsConfig(config.canManage, config.isAdmin),
            "language": {
                "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json"
            }
        });
    };

    const _getColumnsConfig = function (canManage, isAdmin) {
        const columns = [];

        if (canManage) {
            columns.push({
                "data": null,
                "render": function (data, type, row) {
                    const safeAd = _escapeHtml(row.ad);
                    const safeSoyad = _escapeHtml(row.soyad);

                    // Admin: Düzenle + Detay + Sil (tam yetki).
                    // DepartmanYoneticisi (isAdmin=false): Düzenle YOK, bunun
                    // yerine SADECE aktif/pasif yapan bir buton + Detay + Sil.
                    const duzenleVeyaDurumButonu = isAdmin
                        ? `<a href="/Home/Edit/${row.id}" class="btn btn-admin-outline btn-sm">Düzenle</a>`
                        : `<button type="button" class="btn btn-admin-outline btn-sm btn-toggle-status" data-id="${row.id}" data-current="${row.isActive}">
                               ${row.isActive ? 'Pasife Al' : 'Aktife Al'}
                           </button>`;

                    return `
                        <div class="d-flex gap-1 ps-2 flex-wrap">
                            ${duzenleVeyaDurumButonu}
                            <button type="button" class="btn btn-admin-outline btn-sm btn-detay" data-id="${row.id}">Detay</button>
                            <button type="button" class="btn btn-admin-danger-soft btn-sm" 
                                    data-bs-toggle="modal" data-bs-target="#deleteModal" 
                                    data-id="${row.id}" data-name="${safeAd} ${safeSoyad}">Sil</button>
                        </div>
                    `;
                },
                "orderable": false,
                "searchable": false
            });
        }

        columns.push(
            {
                "data": null,
                "render": function (data, type, row) {
                    const ad = row.ad || "";
                    const soyad = row.soyad || "";
                    const imgSrc = _resolveImagePath(row.profileImagePath, ad, soyad);

                    const clickable = canManage ? '' : ' role="button" style="cursor:pointer"';
                    const detayAttr = canManage ? '' : ` data-id="${row.id}"`;

                    return `
                        <div class="d-flex align-items-center gap-2 ${canManage ? '' : 'btn-detay-row'}"${clickable}${detayAttr}>
                            <img src="${imgSrc}" class="rounded-circle shadow-sm" style="width: 36px; height: 36px; object-fit: cover;" alt="Profil">
                            <span class="fw-semibold">${_escapeHtml(ad)} ${_escapeHtml(soyad)}</span>
                        </div>
                    `;
                }
            },
            {
                "data": "email",
                "render": function (data) {
                    return `<span class="text-muted">${_escapeHtml(data)}</span>`;
                }
            },
            {
                "data": "departmanAd",
                "render": function (data) {
                    const dept = data || "Belirtilmemiş";
                    return `<span class="dept-pill">${_escapeHtml(dept)}</span>`;
                }
            },
            {
                "data": "kayitTarihi",
                "render": function (data) {
                    return _escapeHtml(data);
                }
            },
            {
                "data": "isActive",
                "className": "text-end pe-4",
                "render": function (data) {
                    return data
                        ? '<span class="badge bg-success bg-opacity-10 text-success border border-success">Aktif</span>'
                        : '<span class="badge bg-danger bg-opacity-10 text-danger border border-danger">Pasif</span>';
                }
            }
        );

        return columns;
    };

    const _bindEvents = function (config) {
        _$table.find('tbody').on('click', '.btn-detay, .btn-detay-row', function () {
            const userId = $(this).data('id');
            const detayModal = new bootstrap.Modal(document.querySelector(config.modalSelector));

            $.getJSON(`/Home/GetDetailJson/${encodeURIComponent(userId)}`)
                .done(function (data) {
                    const fallbackSrc = _resolveImagePath(data.profileImagePath, data.email, "");

                    $('#modalProfileImage').attr('src', fallbackSrc);
                    $('#modalAdSoyadTitle').text(data.adSoyad);
                    $('#modalDepartmanBadge').text(data.departmanAd);

                    $('#modalAdSoyad').text(data.adSoyad);
                    $('#modalEmail').text(data.email);
                    $('#modalDepartman').text(data.departmanAd);
                    $('#modalKayitTarihi').text(data.kayitTarihi);
                    $('#modalDurum').text(data.isActive ? "Aktif" : "Pasif");

                    detayModal.show();
                })
                .fail(function () {
                    console.error("HATA: Kullanıcı detayı alınamadı, ID:", userId);
                    alert("Kullanıcı detayları getirilirken bir hata oluştu.");
                });
        });

        // YENİ: "Aktif/Pasif Yap" butonu — sadece DepartmanYoneticisi'ne görünür.
        _$table.find('tbody').on('click', '.btn-toggle-status', function () {
            const id = $(this).data('id');
            const mevcutDurum = $(this).data('current');
            _submitStatusToggle(id, !mevcutDurum);
        });

        const deleteModal = document.querySelector(config.deleteModalSelector);
        if (deleteModal) {
            deleteModal.addEventListener('show.bs.modal', event => {
                const button = event.relatedTarget;
                document.getElementById('deleteModalUserName').textContent = button.getAttribute('data-name');
                document.getElementById('deleteModalForm').action = '/Home/Delete/' + encodeURIComponent(button.getAttribute('data-id'));
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