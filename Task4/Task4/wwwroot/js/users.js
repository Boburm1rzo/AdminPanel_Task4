    document.addEventListener('DOMContentLoaded', function() {
            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        });

    document.getElementById("selectAll")?.addEventListener("change", function () {
            const visibleRows = Array.from(document.querySelectorAll("#usersTable tbody tr")).filter(row => row.style.display !== 'none');
            visibleRows.forEach(row => {
                const checkbox = row.querySelector(".user-checkbox");
    if (checkbox) checkbox.checked = this.checked;
            });
        });

    document.getElementById("statusFilter")?.addEventListener("change", function () {
            const filterValue = this.value;
    const rows = document.querySelectorAll("#usersTable tbody tr");

            rows.forEach(row => {
                const status = row.getAttribute("data-status");
    if (filterValue === "" || status === filterValue) {
        row.style.display = "";
                } else {
        row.style.display = "none";
    const checkbox = row.querySelector(".user-checkbox");
    if (checkbox) checkbox.checked = false;
                }
            });

    document.getElementById("selectAll").checked = false;
        });

    function deleteSelected() {
            const selected = document.querySelectorAll(".user-checkbox:checked");
    if (selected.length === 0) return;

    const form = document.createElement('form');
    form.method = 'post';
    form.action = '?handler=DeleteMultiple';

            selected.forEach(cb => {
                const input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'selectedUserIds';
    input.value = cb.value;
    form.appendChild(input);
            });

    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    if (token) {
                const tokenInput = document.createElement('input');
    tokenInput.type = 'hidden';
    tokenInput.name = '__RequestVerificationToken';
    tokenInput.value = token.value;
    form.appendChild(tokenInput);
            }

    document.body.appendChild(form);
    form.submit();
        }


    function clearSelection() {
        document.querySelectorAll(".user-checkbox").forEach(cb => cb.checked = false);
    document.getElementById("selectAll").checked = false;
        }

        document.querySelectorAll(".user-checkbox").forEach(cb => {
        cb.addEventListener("change", function () {
            const visibleCheckboxes = Array.from(document.querySelectorAll(".user-checkbox")).filter(checkbox => {
                const row = checkbox.closest('tr');
                return row && row.style.display !== 'none';
            });
            const allVisibleChecked = visibleCheckboxes.length > 0 && visibleCheckboxes.every(checkbox => checkbox.checked);
            document.getElementById("selectAll").checked = allVisibleChecked;
        });
        });