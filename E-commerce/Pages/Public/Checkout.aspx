<%@ Page Title="Caisse" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Checkout.aspx.cs" Inherits="Ecommerce.Pages.Public.Checkout" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .checkout-container {
                max-width: 800px;
                margin: 2rem auto;
                display: grid;
                grid-template-columns: 1fr 300px;
                gap: 2rem;
            }

            .section-title {
                margin-bottom: 1.5rem;
                padding-bottom: 0.5rem;
                border-bottom: 1px solid var(--glass-border);
            }

            .form-group {
                margin-bottom: 1rem;
            }

            .form-group label {
                display: block;
                margin-bottom: 0.5rem;
                color: var(--text-muted);
            }

            .form-control {
                width: 100%;
                padding: 0.75rem;
                background: rgba(255, 255, 255, 0.05);
                border: 1px solid var(--glass-border);
                border-radius: 8px;
                color: white;
                font-size: 1rem;
            }

            .order-summary {
                background: var(--secondary-bg);
                padding: 1.5rem;
                border-radius: 12px;
                border: 1px solid var(--glass-border);
                height: fit-content;
            }

            .summary-row {
                display: flex;
                justify-content: space-between;
                margin-bottom: 0.5rem;
            }

            .total-row {
                margin-top: 1rem;
                padding-top: 1rem;
                border-top: 1px solid var(--glass-border);
                font-weight: 700;
                font-size: 1.2rem;
                color: var(--accent);
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <div class="checkout-container">
            <!-- Form -->
            <div class="card">
                <h2 class="section-title">Adresse de Livraison</h2>

                <asp:Panel ID="pnlError" runat="server" Visible="false" Style="color: #fca5a5; margin-bottom: 1rem;">
                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="form-group">
                    <label>Rue</label>
                    <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" Required="true"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Ville</label>
                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" Required="true"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Code Postal</label>
                    <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" Required="true"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Pays</label>
                    <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control" Text="France"></asp:TextBox>
                </div>

                <h2 class="section-title" style="margin-top: 2rem;">Paiement</h2>
                <div class="form-group">
                    <p style="color: var(--text-muted);">Paiement à la livraison (Cash on Delivery) pour cette démo.</p>
                </div>
            </div>

            <!-- Summary -->
            <div class="order-summary">
                <h3>Récapitulatif</h3>
                <div style="margin: 1rem 0;">
                    <asp:Repeater ID="rptSummary" runat="server">
                        <ItemTemplate>
                            <div class="summary-row">
                                <span style="font-size:0.9rem;">
                                    <%# Eval("Quantity") %>x <%# Eval("Name") %>
                                </span>
                                <span>
                                    <%# Eval("Total", "{0:C}" ) %>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="summary-row total-row">
                    <span>Total</span>
                    <asp:Label ID="lblTotal" runat="server"></asp:Label>
                </div>

                <asp:Button ID="btnPlaceOrder" runat="server" Text="Confirmer la commande" CssClass="btn btn-primary"
                    Style="width: 100%; margin-top: 1.5rem;" OnClick="btnPlaceOrder_Click" />
            </div>
        </div>

    </asp:Content>