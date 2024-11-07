$(document).ready(function () {
    // Hide error messages initially
    function showSpinner() {
        $('#loadingSpinner').show();
    }

    function hideSpinner() {
        $('#loadingSpinner').hide();
    }
    function showError(message) {
        $('#errorAlert').text(message).removeClass('d-none');
    }
    function hideError() {
        $('#errorAlert').addClass('d-none');
    }

    let currentPage = 1;
    const itemsPerPage = 10;

    // Function to fetch orders with pagination
    function fetchOrders(page = 1) {
        hideError();
        showSpinner(); // Show spinner at the start


        $.get('/Dashboard/GetOrders', function (orders) {
            hideSpinner(); 

            if (!orders || orders.length === 0) {
                $('#ordersTableBody').html("<tr><td colspan='7'>No orders available.</td></tr>");
                $('#paginationControls').html('');
                return;
            }

            // Handle pagination
            const totalPages = Math.ceil(orders.length / itemsPerPage);
            const paginatedOrders = orders.slice((page - 1) * itemsPerPage, page * itemsPerPage);

            // Render paginated orders in the table
            let tableContent = '';
            paginatedOrders.forEach(order => {
                tableContent += `<tr>
                    <td>${order.orderNo || 'N/A'}</td>
                    <td>${order.bookerName || 'N/A'}</td>
                    <td>${order.phone || 'N/A'}</td>
                    <td>${order.status || 'N/A'}</td>
                    <td>${order.orderType || 'N/A'}</td>
                    <td>${order.address || 'N/A'}</td>
                    <td>
                        <button class="btn btn-info" onclick="viewOrder('${order.orderNo}')">Details</button>
                        <button class="btn btn-primary" onclick="openStatusModal('${order.orderNo}', '${order.status}')">Update Status</button>
                    </td>
                </tr>`;
            });
            $('#ordersTableBody').html(tableContent);

            // Render pagination controls
            let paginationContent = '';
            for (let i = 1; i <= totalPages; i++) {
                paginationContent += `<li class="page-item ${i === page ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                </li>`;
            }
            $('#paginationControls').html(paginationContent);
        }).fail(function () {
            showError("Error fetching orders from the server.");
        });
    }

    // Change page function
    window.changePage = function (page) {
        currentPage = page;
        fetchOrders(page);
    }

    // Function to open status modal and set order data
    window.openStatusModal = function (orderNo, currentStatus) {
        $('#orderDetailModal').data('orderNo', orderNo); // Set orderNo in modal's data
        $('#statusChange').val(currentStatus); // Set current status in dropdown
        $('#orderDetailModal').modal('show'); // Show modal
    }

    // Update order status button click event
    $('#updateStatusBtn').click(function () {
        const orderNo = $('#orderDetailModal').data('orderNo');
        const status = $('#statusChange').val();

        if (confirm(`Are you sure you want to change the status of Order ${orderNo} to ${status}?`)) {
            showSpinner(); // Show spinner before updating
            $.post('/Dashboard/UpdateOrderStatus', { orderNo, status })
                .done(() => {
                    alert("Order status updated!");
                    fetchOrders(); // Refresh orders table
                    $('#orderDetailModal').modal('hide');
                })
                .fail(function () {
                    alert("Failed to update status.");
                }).always(function () {
                    hideSpinner(); // Hide spinner after operation completes
                });
        }
    });

    // Search functionality
    $('#searchBtn').click(function () {
        const searchTerm = $('#searchBar').val();
        const searchType = $('#searchType').val();
        showSpinner(); // Show spinner during search

        fetchFilteredOrders(searchType, searchTerm)
            .always(() => hideSpinner()); // Hide spinner when done
    });

    // Filter by status functionality
    $('#filterStatus').change(function () {
        const status = $(this).val();
        showSpinner(); // Show spinner during filter operation

        fetchFilteredOrders(null, '', status)
            .always(() => hideSpinner()); // Hide spinner when done
    });

    // Function to fetch orders with search and filter parameters
    function fetchFilteredOrders(searchType = null, searchTerm = '', status = 'all') {
        $.get('/Dashboard/GetFilteredOrders', { searchType, searchTerm, status }, function (orders) {
            if (!orders || orders.length === 0) {
                $('#ordersTableBody').html("<tr><td colspan='7'>No orders available for selected filters.</td></tr>");
                return;
            }

            let tableContent = '';
            orders.forEach(order => {
                tableContent += `<tr>
                    <td>${order.orderNo}</td>
                    <td>${order.bookerName}</td>
                    <td>${order.phone}</td>
                    <td>${order.status}</td>
                    <td>${order.orderType}</td>
                    <td>${order.address}</td>
                    <td>
                        <button class="btn btn-info" onclick="viewOrder('${order.orderNo}')">Details</button>
                        <button class="btn btn-primary" onclick="openStatusModal('${order.orderNo}', '${order.status}')">Update Status</button>
                    </td>
                </tr>`;
            });
            $('#ordersTableBody').html(tableContent);
        }).fail(function () {
            showError("Error fetching filtered orders from the server.");
        });
    }

    // View order details in modal
    window.viewOrder = function (orderId) {
        $.get(`/Dashboard/GetOrderById?orderNo=${orderId}`, function (order) {
            if (order) {
                $('#modalOrderNo').text(order.orderNo);
                $('#modalOrderStatus').text(order.status);
                $('#modalQuantity').text(order.quantity || 'N/A');
                $('#modalCurrentRate').text(order.currentRate || 'N/A');
                $('#modalTotalPrice').text(order.totalPrice || 'N/A');
                $('#modalBookerName').text(order.bookerName);
                $('#modalPhone').text(order.phone);
                $('#modalAddress').text(order.address);

                $('#viewOrderModal').modal('show');
            } else {
                alert("Order details could not be found.");
            }
        }).fail(function () {
            alert("Failed to fetch order details.");
        });
    };

    // Initial fetch of orders
    fetchOrders();
});
