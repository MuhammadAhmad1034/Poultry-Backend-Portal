$(document).ready(function () {
    // Polling function to refresh orders periodically
    setInterval(() => {
        fetchOrders();
    }, 300000); // 5 minutes
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
    const itemsPerPage = 8;

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
    // Function to fetch and display orders based on order type
    // Function to fetch and display orders based on order type
    function fetchOrdersByType(orderType) {
        $('#ordersTableBody').empty(); // Clear table for new data
        showSpinner();

        $.get(`/Dashboard/GetOrdersByType?type=${orderType}`, function (orders) {
            if (!orders || orders.length === 0) {
                $('#ordersTableBody').html("<tr><td colspan='7'>No orders available for this type.</td></tr>");
                return;
            }

            // Render orders for selected type in the table
            let tableContent = '';
            orders.forEach(order => {
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
        })
            .fail(function () {
                console.error("Error fetching orders of type " + orderType);
                $('#ordersTableBody').html("<tr><td colspan='7'>Error fetching orders. Please try again.</td></tr>");
            })
            .always(() => hideSpinner());
    }

    // Function to set the clicked button as active
    function setActiveButton(buttonId) {
        // Remove the 'active' class from all buttons
        $('#broilerOrdersBtn, #eggOrdersBtn, #hatcheryOrdersBtn').removeClass('active');

        // Add the 'active' class to the clicked button
        $(`#${buttonId}`).addClass('active');
    }

    // Event listeners for main action buttons
    $('#broilerOrdersBtn').click(function () {
        setActiveButton('broilerOrdersBtn'); // Activate button
        fetchOrdersByType('Chicken/Broiler'); // Fetch orders of type 'Broiler'
    });

    $('#eggOrdersBtn').click(function () {
        setActiveButton('eggOrdersBtn'); // Activate button

        fetchOrdersByType('Egg/Golden'); // Fetch orders of type 'Egg'
    });

    $('#hatcheryOrdersBtn').click(function () {
        setActiveButton('hatcheryOrdersBtn'); // Activate button
        fetchOrdersByType('Hatchery'); // Fetch orders of type 'Hatchery'
    });


    // Update order status button click event
    //$('#updateStatusBtn').click(function () {
    //    const orderNo = $('#orderDetailModal').data('orderNo');
    //    const status = $('#statusChange').val();

    //    if (confirm(`Are you sure you want to change the status of Order ${orderNo} to ${status}?`)) {
    //        showSpinner(); // Show spinner before updating
    //        $.post('/Dashboard/UpdateOrderStatus', { orderNo, status })
    //            .done(() => {
    //                alert("Order status updated!");
    //                fetchOrders(); // Refresh orders table
    //                $('#orderDetailModal').modal('hide');
    //            })
    //            .fail(function () {
    //                alert("Failed to update status.");
    //            }).always(function () {
    //                hideSpinner(); // Hide spinner after operation completes
    //            });
    //    }
    //});
    $('#updateStatusBtn').click(function () {
        const orderNo = $('#orderDetailModal').data('orderNo');
        const status = $('#statusChange').val();
        $('#confirmationModal').modal('show'); // Show confirmation modal

        // When user confirms update
        $('#confirmUpdateStatusBtn').off('click').on('click', function () {
            $('#confirmationModal').modal('hide');
            showSpinner();

            $.post('/Dashboard/UpdateOrderStatus', { orderNo, status })
                .done(() => {
                    showToast("Order status updated!", "success"); // Use toast for feedback
                    fetchOrders(); // Refresh orders table
                    $('#orderDetailModal').modal('hide');
                })
                .fail(function () {
                    showToast("Failed to update status.", "error");
                })
                .always(() => hideSpinner());
        });
    });
    // Search functionality
    $('#searchBtn').click(function () {
        const searchTerm = $('#searchBar').val();
        const searchType = $('#searchType').val();
        //showSpinner(); // Show spinner during search

        fetchFilteredOrders(searchType, searchTerm);
        /*.always(() => hideSpinner()); // Hide spinner when done*/
    });

    // Filter by status functionality
    $('#filterStatus').change(function () {
        const status = $(this).val();


        fetchFilteredOrders(null, '', status);

    });

    // Function to fetch orders with search and filter parameters
    function fetchFilteredOrders(searchType = null, searchTerm = '', status = 'all') {
        showSpinner();
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
        }).always(function () {
            hideSpinner(); // Hide spinner after operation completes
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
function showToast(message, type) {
    const toastEl = document.getElementById('liveToast');
    const toastMessageEl = document.getElementById('toastMessage');

    toastMessageEl.textContent = message;
    toastEl.classList.remove('bg-success', 'bg-danger');

    if (type === 'success') {
        toastEl.classList.add('bg-success', 'text-white');
    } else if (type === 'error') {
        toastEl.classList.add('bg-danger', 'text-white');
    }

    const toast = new bootstrap.Toast(toastEl);
    toast.show();
}
