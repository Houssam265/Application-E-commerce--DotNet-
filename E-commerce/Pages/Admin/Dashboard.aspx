<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="Ecommerce.Pages.Admin.Dashboard" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
        <style>
            .stats-row {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
                gap: 1.5rem;
                margin-bottom: 2.5rem;
            }

            .stat-card {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                padding: 2rem;
                border-radius: 16px;
                position: relative;
                overflow: hidden;
                transition: all 0.3s ease;
                cursor: pointer;
            }

            .stat-card:hover {
                transform: translateY(-4px);
                box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
                border-color: #cbd5e1;
            }

            .stat-card::before {
                content: '';
                position: absolute;
                top: -50%;
                right: -50%;
                width: 200px;
                height: 200px;
                background: radial-gradient(circle, rgba(59, 130, 246, 0.15) 0%, transparent 70%);
                transition: all 0.5s ease;
            }

            .stat-card:hover::before {
                top: -30%;
                right: -30%;
            }

            .stat-card-header {
                display: flex;
                justify-content: space-between;
                align-items: flex-start;
                margin-bottom: 1rem;
            }

            .stat-icon {
                font-size: 2.5rem;
                width: 60px;
                height: 60px;
                display: flex;
                align-items: center;
                justify-content: center;
                border-radius: 12px;
                background: rgba(59, 130, 246, 0.1);
                transition: all 0.3s ease;
            }

            .stat-card:hover .stat-icon {
                transform: scale(1.1) rotate(5deg);
            }

            .stat-card.orders .stat-icon { background: rgba(59, 130, 246, 0.15); }
            .stat-card.revenue .stat-icon { background: rgba(16, 185, 129, 0.15); }
            .stat-card.products .stat-icon { background: rgba(139, 92, 246, 0.15); }
            .stat-card.users .stat-icon { background: rgba(245, 158, 11, 0.15); }

            .stat-label {
                color: #64748b;
                font-size: 0.875rem;
                font-weight: 500;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                margin-bottom: 0.5rem;
            }

            .stat-value {
                font-size: 2.5rem;
                font-weight: 700;
                color: #1e293b;
                margin-bottom: 0.5rem;
                font-family: 'Outfit', sans-serif;
            }

            .stat-change {
                font-size: 0.875rem;
                font-weight: 600;
                display: flex;
                align-items: center;
                gap: 0.25rem;
            }

            .stat-change.positive {
                color: #10b981;
            }

            .stat-change.negative {
                color: #ef4444;
            }

            .stat-change.neutral {
                color: #6366f1;
            }

            .chart-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 2rem;
                height: 400px;
                position: relative;
                transition: all 0.3s ease;
            }

            .chart-container:hover {
                border-color: #cbd5e1;
                box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
            }

            .chart-grid {
                display: grid;
                grid-template-columns: 2fr 1fr;
                gap: 1.5rem;
                margin-bottom: 1.5rem;
            }

            @media (max-width: 968px) {
                .chart-grid {
                    grid-template-columns: 1fr;
                }
            }

            .chart-title {
                font-size: 1.25rem;
                font-weight: 600;
                margin-bottom: 1.5rem;
                color: #1e293b;
                display: flex;
                align-items: center;
                gap: 0.5rem;
            }

            .quick-actions {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 2rem;
                margin-top: 2rem;
            }

            .quick-actions h3 {
                font-size: 1.5rem;
                font-weight: 700;
                margin-bottom: 1.5rem;
                color: #1e293b;
                font-family: 'Outfit', sans-serif;
            }

            .actions-grid {
                display: flex;
                gap: 1rem;
                flex-wrap: wrap;
            }

            .action-btn {
                padding: 1rem 1.5rem;
                border-radius: 12px;
                font-weight: 600;
                font-size: 0.95rem;
                border: none;
                cursor: pointer;
                transition: all 0.3s ease;
                display: inline-flex;
                align-items: center;
                gap: 0.75rem;
                text-decoration: none;
            }

            .action-btn.primary {
                background: linear-gradient(135deg, #3b82f6, #8b5cf6);
                color: white;
            }

            .action-btn.secondary {
                background: #f1f5f9;
                color: #64748b;
                border: 1px solid #e2e8f0;
            }

            .action-btn.special {
                background: transparent;
                color: #f59e0b;
                border: 2px dashed #f59e0b;
            }

            .action-btn:hover {
                transform: translateY(-2px);
                box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3);
            }

            .action-btn.primary:hover {
                box-shadow: 0 8px 16px rgba(59, 130, 246, 0.4);
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div style="margin-bottom: 2rem;">
            <h1><i class="fas fa-chart-bar"></i> Tableau de bord</h1>
            <p style="color: #64748b; margin-top: 0.5rem;">Vue d'ensemble de la boutique et statistiques en temps réel</p>
        </div>

        <!-- Stats Cards -->
        <div class="stats-row">
            <div class="stat-card orders">
                <div class="stat-card-header">
                    <div>
                        <div class="stat-label">Commandes Totales</div>
                        <div class="stat-value">
                            <asp:Label ID="lblTotalOrders" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                    <div class="stat-icon"><i class="fas fa-box"></i></div>
                </div>
                <div class="stat-change positive">
                    <i class="fas fa-arrow-up"></i>
                    <span>+12% ce mois</span>
                </div>
            </div>

            <div class="stat-card revenue">
                <div class="stat-card-header">
                    <div>
                        <div class="stat-label">Chiffre d'Affaires</div>
                        <div class="stat-value" style="color: #10b981;">
                            <asp:Label ID="lblRevenue" runat="server" Text="0.00 MAD"></asp:Label>
                        </div>
                    </div>
                    <div class="stat-icon"><i class="fas fa-money-bill-wave"></i></div>
                </div>
                <div class="stat-change positive">
                    <i class="fas fa-arrow-up"></i>
                    <span>+8% ce mois</span>
                </div>
            </div>

            <div class="stat-card products">
                <div class="stat-card-header">
                    <div>
                        <div class="stat-label">Produits Actifs</div>
                        <div class="stat-value">
                            <asp:Label ID="lblTotalProducts" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                    <div class="stat-icon"><i class="fas fa-tags"></i></div>
                </div>
                <div class="stat-change neutral">
                    <i class="fas fa-layer-group"></i>
                    <span>4 catégories</span>
                </div>
            </div>

            <div class="stat-card users">
                <div class="stat-card-header">
                    <div>
                        <div class="stat-label">Utilisateurs</div>
                        <div class="stat-value">
                            <asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                    <div class="stat-icon"><i class="fas fa-users"></i></div>
                </div>
                <div class="stat-change positive">
                    <i class="fas fa-user-plus"></i>
                    <span>+3 cette semaine</span>
                </div>
            </div>
        </div>

        <!-- Charts Row -->
        <div class="chart-grid">
            <div class="chart-container">
                <div class="chart-title"><i class="fas fa-chart-line"></i> Ventes des 7 derniers jours</div>
                <canvas id="salesChart"></canvas>
            </div>

            <div class="chart-container">
                <div class="chart-title"><i class="fas fa-chart-pie"></i> Commandes par Statut</div>
                <canvas id="ordersChart"></canvas>
            </div>
        </div>

        <!-- Products Chart -->
        <div class="chart-container" style="margin-bottom: 2rem;">
            <div class="chart-title"><i class="fas fa-trophy"></i> Top 5 Produits</div>
            <canvas id="productsChart"></canvas>
        </div>

        <!-- Actions Rapides -->
        <div class="quick-actions">
            <h3><i class="fas fa-bolt"></i> Actions Rapides</h3>
            <div class="actions-grid">
                <a href="Products.aspx" class="action-btn primary">
                    <i class="fas fa-plus-circle"></i>
                    <span>Gérer les produits</span>
                </a>
                <a href="Categories.aspx" class="action-btn secondary">
                    <i class="fas fa-tags"></i>
                    <span>Gérer les catégories</span>
                </a>
                <a href="Orders.aspx" class="action-btn secondary">
                    <i class="fas fa-shopping-bag"></i>
                    <span>Voir les commandes</span>
                </a>
                <a href="Users.aspx" class="action-btn secondary">
                    <i class="fas fa-users"></i>
                    <span>Gérer les utilisateurs</span>
                </a>
            </div>
        </div>

        <!-- Hidden fields pour les données des graphiques -->
        <asp:HiddenField ID="hfSalesData" runat="server" />
        <asp:HiddenField ID="hfOrdersByStatus" runat="server" />
        <asp:HiddenField ID="hfTopProducts" runat="server" />

        <script>
            // Chart configuration
            const chartColors = {
                primary: '#6366f1',
                success: '#10b981',
                warning: '#f59e0b',
                danger: '#ef4444',
                info: '#3b82f6',
                purple: '#8b5cf6'
            };

            const chartDefaults = {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: {
                            color: '#94a3b8',
                            font: { family: 'Inter', size: 12 }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { color: 'rgba(148, 163, 184, 0.1)' },
                        ticks: { color: '#94a3b8' }
                    },
                    y: {
                        grid: { color: 'rgba(148, 163, 184, 0.1)' },
                        ticks: { color: '#94a3b8' }
                    }
                }
            };

            // Sales Line Chart - Données réelles
            const salesData = JSON.parse(document.getElementById('<%= hfSalesData.ClientID %>').value || '[0,0,0,0,0,0,0]');
            const dayLabels = [];
            const today = new Date();
            for (let i = 6; i >= 0; i--) {
                const date = new Date(today);
                date.setDate(date.getDate() - i);
                const dayNames = ['Dim', 'Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam'];
                dayLabels.push(dayNames[date.getDay()]);
            }
            
            new Chart(document.getElementById('salesChart'), {
                type: 'line',
                data: {
                    labels: dayLabels,
                    datasets: [{
                        label: 'Ventes (MAD)',
                        data: salesData,
                        borderColor: chartColors.primary,
                        backgroundColor: 'rgba(99, 102, 241, 0.1)',
                        tension: 0.4,
                        fill: true,
                        borderWidth: 2
                    }]
                },
                options: {
                    ...chartDefaults,
                    plugins: {
                        ...chartDefaults.plugins,
                        tooltip: {
                            callbacks: {
                                label: (context) => context.parsed.y + ' MAD'
                            }
                        }
                    }
                }
            });

            // Orders Doughnut Chart - Données réelles
            const ordersByStatus = JSON.parse(document.getElementById('<%= hfOrdersByStatus.ClientID %>').value || '[0,0,0,0]');
            new Chart(document.getElementById('ordersChart'), {
                type: 'doughnut',
                data: {
                    labels: ['En attente', 'Expédié', 'Livré', 'Annulé'],
                    datasets: [{
                        data: ordersByStatus,
                        backgroundColor: [
                            chartColors.warning,
                            chartColors.info,
                            chartColors.success,
                            chartColors.danger
                        ],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                color: '#94a3b8',
                                font: { family: 'Inter', size: 11 },
                                padding: 15
                            }
                        }
                    }
                }
            });

            // Top Products Bar Chart - Données réelles
            const topProductsData = JSON.parse(document.getElementById('<%= hfTopProducts.ClientID %>').value || '{"labels":["-","-","-","-","-"],"data":[0,0,0,0,0]}');
            new Chart(document.getElementById('productsChart'), {
                type: 'bar',
                data: {
                    labels: topProductsData.labels,
                    datasets: [{
                        label: 'Ventes',
                        data: topProductsData.data,
                        backgroundColor: [
                            chartColors.primary,
                            chartColors.purple,
                            chartColors.info,
                            chartColors.warning,
                            chartColors.success
                        ],
                        borderRadius: 8,
                        borderWidth: 0
                    }]
                },
                options: {
                    ...chartDefaults,
                    indexAxis: 'y',
                    plugins: {
                        legend: { display: false }
                    }
                }
            });
        </script>
    </asp:Content>