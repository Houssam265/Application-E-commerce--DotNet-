<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="Ecommerce.Pages.Admin.Dashboard" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
        <style>
            .dashboard-grid {
                display: grid;
                gap: 1.5rem;
            }

            .stats-row {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                gap: 1.5rem;
                margin-bottom: 2rem;
            }

            .stat-card {
                background: linear-gradient(135deg, var(--secondary-bg) 0%, rgba(99, 102, 241, 0.1) 100%);
                border: 1px solid var(--glass-border);
                padding: 1.5rem;
                border-radius: 16px;
                position: relative;
                overflow: hidden;
            }

            .stat-card::before {
                content: '';
                position: absolute;
                top: 0;
                right: 0;
                width: 100px;
                height: 100px;
                background: radial-gradient(circle, rgba(99, 102, 241, 0.2) 0%, transparent 70%);
            }

            .stat-icon {
                font-size: 2rem;
                margin-bottom: 0.5rem;
                display: inline-block;
            }

            .chart-container {
                background: var(--secondary-bg);
                border: 1px solid var(--glass-border);
                border-radius: 16px;
                padding: 1.5rem;
                height: 350px;
                position: relative;
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
                font-size: 1.1rem;
                font-weight: 600;
                margin-bottom: 1rem;
                color: var(--text-primary);
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <h1>📊 Tableau de bord</h1>
        <p style="color: var(--text-muted); margin-bottom: 2rem;">Vue d'ensemble de la boutique.</p>

        <!-- Stats Cards -->
        <div class="stats-row">
            <div class="stat-card">
                <div class="stat-icon">📦</div>
                <h3 style="color: var(--text-muted); font-size: 0.9rem;">Commandes Totales</h3>
                <div style="font-size: 2.5rem; font-weight: 700; margin-top: 0.5rem;">
                    <asp:Label ID="lblTotalOrders" runat="server" Text="0"></asp:Label>
                </div>
                <div style="color: #10b981; font-size: 0.85rem; margin-top: 0.5rem;">+12% ce mois</div>
            </div>

            <div class="stat-card">
                <div class="stat-icon">💰</div>
                <h3 style="color: var(--text-muted); font-size: 0.9rem;">Chiffre d'Affaires</h3>
                <div style="font-size: 2.5rem; font-weight: 700; margin-top: 0.5rem; color: #10b981;">
                    <asp:Label ID="lblRevenue" runat="server" Text="0.00 €"></asp:Label>
                </div>
                <div style="color: #10b981; font-size: 0.85rem; margin-top: 0.5rem;">+8% ce mois</div>
            </div>

            <div class="stat-card">
                <div class="stat-icon">🏷️</div>
                <h3 style="color: var(--text-muted); font-size: 0.9rem;">Produits Actifs</h3>
                <div style="font-size: 2.5rem; font-weight: 700; margin-top: 0.5rem;">
                    <asp:Label ID="lblTotalProducts" runat="server" Text="0"></asp:Label>
                </div>
                <div style="color: #6366f1; font-size: 0.85rem; margin-top: 0.5rem;">4 catégories</div>
            </div>

            <div class="stat-card">
                <div class="stat-icon">👥</div>
                <h3 style="color: var(--text-muted); font-size: 0.9rem;">Utilisateurs</h3>
                <div style="font-size: 2.5rem; font-weight: 700; margin-top: 0.5rem;">
                    <asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label>
                </div>
                <div style="color: #f59e0b; font-size: 0.85rem; margin-top: 0.5rem;">+3 cette semaine</div>
            </div>
        </div>

        <!-- Charts Row -->
        <div class="chart-grid">
            <div class="chart-container">
                <div class="chart-title">📈 Ventes des 7 derniers jours</div>
                <canvas id="salesChart"></canvas>
            </div>

            <div class="chart-container">
                <div class="chart-title">📊 Commandes par Statut</div>
                <canvas id="ordersChart"></canvas>
            </div>
        </div>

        <!-- Products Chart -->
        <div class="chart-container" style="margin-bottom: 2rem;">
            <div class="chart-title">🏆 Top 5 Produits</div>
            <canvas id="productsChart"></canvas>
        </div>

        <!-- Actions Rapides -->
        <div class="card">
            <h3>Actions Rapides</h3>
            <div style="display: flex; gap: 1rem; margin-top: 1rem; flex-wrap: wrap;">
                <a href="Products.aspx" class="btn btn-primary">➕ Gérer les produits</a>
                <a href="Orders.aspx" class="btn" style="background: rgba(255,255,255,0.1);">📋 Voir les commandes</a>
                <asp:Button ID="btnSeed" runat="server" Text="🎲 Initialiser Données Démo" CssClass="btn"
                    OnClick="btnSeed_Click"
                    style="border: 1px dashed var(--gold); background: transparent; color: var(--gold);" />
            </div>
            <asp:Label ID="lblMsg" runat="server" Visible="false" Style="display:block; margin-top: 1rem;"></asp:Label>
        </div>

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

            // Sales Line Chart
            new Chart(document.getElementById('salesChart'), {
                type: 'line',
                data: {
                    labels: ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim'],
                    datasets: [{
                        label: 'Ventes (€)',
                        data: [1200, 1900, 800, 1500, 2000, 2400, 1800],
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
                                label: (context) => context.parsed.y + ' €'
                            }
                        }
                    }
                }
            });

            // Orders Doughnut Chart
            new Chart(document.getElementById('ordersChart'), {
                type: 'doughnut',
                data: {
                    labels: ['En attente', 'Expédié', 'Livré', 'Annulé'],
                    datasets: [{
                        data: [3, 5, 12, 1],
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

            // Top Products Bar Chart
            new Chart(document.getElementById('productsChart'), {
                type: 'bar',
                data: {
                    labels: ['Montre Chronographe', 'Sac Cuir Premium', 'Bracelet Perles', 'Lunettes Soleil', 'Collier Diamant'],
                    datasets: [{
                        label: 'Ventes',
                        data: [45, 38, 32, 28, 25],
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