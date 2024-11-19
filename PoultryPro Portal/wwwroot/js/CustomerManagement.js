let currentPage = 1;
const itemsPerPage = 10;

async function loadSuppliers(query = '', searchType = 'name', page = 1) {
    currentPage = page;  // Track the current page for pagination

    // Create URL with pagination parameters
    const url = query
        ? `/Customer/SearchSuppliers?query=${query}&searchType=${searchType}&page=${page}&itemsPerPage=${itemsPerPage}`
        : `/Customer/GetSuppliers?page=${page}&itemsPerPage=${itemsPerPage}`;

    try {
        const response = await fetch(url);
        const suppliers = await response.json();

        // Render suppliers in UI
        renderSuppliers(suppliers);

        // Update pagination controls based on data count
        updatePaginationControls();
    } catch (error) {
        console.error("Failed to load suppliers:", error);
    }
}

// Function to update pagination controls in the UI
function updatePaginationControls() {
    const paginationElement = document.getElementById('paginationControls');

    // Clear existing pagination controls
    paginationElement.innerHTML = '';

    // Example logic for updating pagination based on current page
    paginationElement.innerHTML = `
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="loadSuppliers('', 'name', ${currentPage - 1})">Previous</a>
        </li>
        <li class="page-item"><a class="page-link" href="#">Page ${currentPage}</a></li>
        <li class="page-item">
            <a class="page-link" href="#" onclick="loadSuppliers('', 'name', ${currentPage + 1})">Next</a>
        </li>
    `;
}
// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Initial load of suppliers (since it's the default active tab)
    loadSuppliers();

    // Add tab change listeners
    document.getElementById('suppliers-tab').addEventListener('click', () => {
        loadSuppliers();
    });

    document.getElementById('wholesalers-tab').addEventListener('click', () => {
        loadWholesalers();
    });
});

//async function loadSuppliers(query = '', searchType = 'name') {
//    try {
//        const url = query ? `/Customer/SearchSuppliers?query=${query}&searchType=${searchType}` : '/Customer/GetSuppliers';
//        const response = await fetch(url);
//        if (!response.ok) {
//            throw new Error(`HTTP error! status: ${response.status}`);
//        }
//        const suppliers = await response.json();
//        console.log('Loaded suppliers:', suppliers);
//        renderSuppliers(suppliers);
//    } catch (error) {
//        console.error('Error loading suppliers:', error);
//    }
//}

async function loadWholesalers(query = '', searchType = 'name') {
    try {
        const url = query ? `/Customer/SearchWholesalers?query=${query}&searchType=${searchType}` : '/Customer/GetWholesalers';
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const wholesalers = await response.json();
        console.log('Loaded wholesalers:', wholesalers);
        renderWholesalers(wholesalers);
    } catch (error) {
        console.error('Error loading wholesalers:', error);
    }
}


// Add these new functions to handle details view
async function showSupplierDetails(supplierId) {
    try {
        const response = await fetch(`/Customer/GetSupplierDetails/${supplierId}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const supplier = await response.json();

        // Populate modal with supplier details
        const detailsContent = document.getElementById('customerDetailsContent');
        detailsContent.innerHTML = `
            <div class="container">
                <h4>${supplier.name}</h4>
                <div class="row">
                    <div class="col-md-6">
                        <p><strong>Type:</strong> ${supplier.type}</p>
                        <p><strong>City:</strong> ${supplier.city}</p>
                        <p><strong>Contact:</strong> ${supplier.contact}</p>
                        <p><strong>Commission:</strong> ${supplier.commission}%</p>
                    </div>
                    <div class="col-md-6">
                        <h5>Inventory</h5>
                        <p><strong>Eggs:</strong> ${supplier.inventory?.eggs?.quantity || 0} (${supplier.inventory?.eggs?.type || 'N/A'})</p>
                        <p><strong>Broiler:</strong> ${supplier.inventory?.broiler?.quantity || 0} (${supplier.inventory?.broiler?.type || 'N/A'})</p>
                    </div>
                </div>
                
                <h5 class="mt-3">Sheds</h5>
                <div class="table-responsive">
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Type</th>
                                <th>Capacity</th>
                                <th>Current</th>
                                <th>Address</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${supplier.sheds?.map(shed => `
                                <tr>
                                    <td>${shed.id}</td>
                                    <td>${shed.type}</td>
                                    <td>${shed.capacity}</td>
                                    <td>${shed.current}</td>
                                    <td>${shed.address}</td>
                                </tr>
                            `).join('') || '<tr><td colspan="5">No sheds available</td></tr>'}
                        </tbody>
                    </table>
                </div>
                
                <h5 class="mt-3">Recent Orders</h5>
                <div class="table-responsive">
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Date</th>
                                <th>Type</th>
                                <th>Quantity</th>
                                <th>Rate</th>
                                <th>Total</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${supplier.orders?.map(order => `
                                <tr>
                                    <td>${order.id}</td>
                                    <td>${new Date(order.date).toLocaleDateString()}</td>
                                    <td>${order.type}</td>
                                    <td>${order.quantity}</td>
                                    <td>${order.rate}</td>
                                    <td>${order.total}</td>
                                    <td><span class="badge bg-${order.status === 'Completed' ? 'success' : 'warning'}">${order.status}</span></td>
                                </tr>
                            `).join('') || '<tr><td colspan="7">No orders available</td></tr>'}
                        </tbody>
                    </table>
                </div>
            </div>
        `;

        // Show the modal
        const modal = new bootstrap.Modal(document.getElementById('customerDetailModal'));
        modal.show();
    } catch (error) {
        console.error('Error loading supplier details:', error);
    }
}

async function showWholesalerDetails(wholesalerId) {
    try {
        const response = await fetch(`/Customer/GetWholesalerDetails/${wholesalerId}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const wholesaler = await response.json();

        // Populate modal with wholesaler details
        const detailsContent = document.getElementById('customerDetailsContent');
        detailsContent.innerHTML = `
            <div class="container">
                <h4>${wholesaler.name}</h4>
                <div class="row">
                    <div class="col-md-6">
                        <p><strong>City:</strong> ${wholesaler.city}</p>
                        <p><strong>Contact:</strong> ${wholesaler.contact}</p>
                        <p><strong>Total Revenue:</strong> $${wholesaler.totalRevenue.toLocaleString()}</p>
                    </div>
                </div>
                
                <h5 class="mt-3">Current Orders</h5>
                <div class="table-responsive">
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Date</th>
                                <th>Type</th>
                                <th>Quantity</th>
                                <th>Rate</th>
                                <th>Total</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${wholesaler.currentOrders?.map(order => `
                                <tr>
                                    <td>${order.id}</td>
                                    <td>${new Date(order.date).toLocaleDateString()}</td>
                                    <td>${order.type}</td>
                                    <td>${order.quantity}</td>
                                    <td>${order.rate}</td>
                                    <td>${order.total}</td>
                                    <td><span class="badge bg-${order.status === 'Completed' ? 'success' : 'warning'}">${order.status}</span></td>
                                </tr>
                            `).join('') || '<tr><td colspan="7">No current orders</td></tr>'}
                        </tbody>
                    </table>
                </div>
                
                <h5 class="mt-3">Order History</h5>
                <div class="table-responsive">
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Date</th>
                                <th>Type</th>
                                <th>Quantity</th>
                                <th>Rate</th>
                                <th>Total</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${wholesaler.orderHistory?.map(order => `
                                <tr>
                                    <td>${order.id}</td>
                                    <td>${new Date(order.date).toLocaleDateString()}</td>
                                    <td>${order.type}</td>
                                    <td>${order.quantity}</td>
                                    <td>${order.rate}</td>
                                    <td>${order.total}</td>
                                    <td><span class="badge bg-${order.status === 'Completed' ? 'success' : 'warning'}">${order.status}</span></td>
                                </tr>
                            `).join('') || '<tr><td colspan="7">No order history</td></tr>'}
                        </tbody>
                    </table>
                </div>
            </div>
        `;

        // Show the modal
        const modal = new bootstrap.Modal(document.getElementById('customerDetailModal'));
        modal.show();
    } catch (error) {
        console.error('Error loading wholesaler details:', error);
    }
}

function renderSuppliers(suppliers) {
    const supplierList = document.getElementById('suppliersList');
    if (!supplierList) {
        console.error('suppliersList element not found');
        return;
    }

    supplierList.innerHTML = suppliers.map(supplier => `
        <div class="col-md-6 col-lg-4 mb-3">
            <div class="card h-100">
                <div class="card-body">
                    <h5 class="card-title">${supplier.name || 'Unnamed Supplier'}</h5>
                    <span class="badge bg-info mb-2">${supplier.type || 'N/A'}</span>
                    <p class="card-text"><i class="fas fa-map-marker-alt"></i> ${supplier.city || 'No Location'}</p>
                    <button class="btn btn-primary" onclick="showSupplierDetails('${supplier.id}')">View Details</button>
                </div>
            </div>
        </div>
    `).join('');
}

function renderWholesalers(wholesalers) {
    const wholesalerList = document.getElementById('wholesalersList');
    if (!wholesalerList) {
        console.error('wholesalersList element not found');
        return;
    }

    wholesalerList.innerHTML = wholesalers.map(wholesaler => `
        <div class="col-md-6 col-lg-4 mb-3">
            <div class="card h-100">
                <div class="card-body">
                    <h5 class="card-title">${wholesaler.name || 'Unnamed Wholesaler'}</h5>
                    <p class="card-text"><i class="fas fa-map-marker-alt"></i> ${wholesaler.city || 'No Location'}</p>
                    <button class="btn btn-primary" onclick="showWholesalerDetails('${wholesaler.id}')">View Details</button>
                </div>
            </div>
        </div>
    `).join('');
}

// Search button click handler
document.getElementById('searchBtn')?.addEventListener('click', () => {
    const query = document.getElementById('searchBar').value;
    const searchType = document.getElementById('searchType').value;
    const activeTab = document.querySelector('.tab-pane.active');
    if (activeTab.id === 'suppliers') {
        loadSuppliers(query, searchType);
    } else {
        loadWholesalers(query, searchType);
    }
});