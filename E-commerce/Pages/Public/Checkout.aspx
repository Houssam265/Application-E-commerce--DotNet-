<%@ Page Title="Caisse" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Checkout.aspx.cs" Inherits="Ecommerce.Pages.Public.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .checkout-container {
            max-width: 1000px;
            margin: 2rem auto;
            display: grid;
            grid-template-columns: 1fr 400px;
            gap: 2rem;
        }

        .checkout-header {
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--border-color);
        }

        .section-title {
            margin-bottom: 1.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid var(--primary-color);
            color: var(--text-dark);
        }

        .form-group {
            margin-bottom: 1.5rem;
        }

        .form-group label {
            display: block;
            margin-bottom: 0.5rem;
            color: var(--text-dark);
            font-weight: 600;
        }

        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
            font-size: 14px;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary-color);
            box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
        }

        .order-summary {
            background: var(--bg-white);
            padding: 2rem;
            border-radius: 10px;
            border: 1px solid var(--border-color);
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            height: fit-content;
            position: sticky;
            top: 100px;
        }

        .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 1rem;
            padding-bottom: 1rem;
            border-bottom: 1px solid var(--border-color);
        }

        .summary-row:last-child {
            border-bottom: none;
        }

        .summary-item {
            font-size: 0.9rem;
            color: var(--text-light);
        }

        .summary-price {
            font-weight: 600;
            color: var(--text-dark);
        }

        .total-row {
            margin-top: 1rem;
            padding-top: 1rem;
            border-top: 2px solid var(--primary-color);
            font-weight: 700;
            font-size: 1.3rem;
            color: var(--primary-color);
        }

        .payment-method {
            padding: 1rem;
            border: 2px solid var(--border-color);
            border-radius: 5px;
            margin-bottom: 1rem;
            cursor: pointer;
            transition: all 0.3s;
        }

        .payment-method:hover {
            border-color: var(--primary-color);
        }

        .payment-method.selected {
            border-color: var(--primary-color);
            background: rgba(40, 167, 69, 0.05);
        }

        .bank-transfer-fields {
            margin-top: 1.5rem;
            padding: 1.5rem;
            background: #f8f9fa;
            border-radius: 8px;
            border: 1px solid #e2e8f0;
            display: none;
        }

        .bank-transfer-fields.show {
            display: block;
            animation: slideDown 0.3s ease;
        }

        @keyframes slideDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        @media (max-width: 768px) {
            .checkout-container {
                grid-template-columns: 1fr;
            }

            .order-summary {
                position: static;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="checkout-header">
            <h1><i class="fas fa-shopping-bag" style="color: var(--primary-color);"></i> Finaliser la commande</h1>
        </div>

        <asp:Panel ID="pnlEmptyCart" runat="server" Visible="false">
            <div style="text-align: center; padding: 4rem;">
                <h2>Votre panier est vide</h2>
                <p style="color: var(--text-light); margin-bottom: 2rem;">Ajoutez des produits avant de passer commande.</p>
                <a href="Cart.aspx" class="btn btn-primary">Retour au panier</a>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCheckout" runat="server">
            <div class="checkout-container">
                <!-- Form -->
                <div>
                    <div class="card">
                        <h2 class="section-title"><i class="fas fa-map-marker-alt"></i> Adresse de Livraison</h2>

                        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                            <i class="fas fa-exclamation-circle"></i> <asp:Literal ID="litError" runat="server"></asp:Literal>
                        </asp:Panel>

                        <div class="form-group">
                            <label>Nom complet *</label>
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" required></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Rue *</label>
                            <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" required></asp:TextBox>
                        </div>

                        <div style="display: grid; grid-template-columns: 2fr 1fr; gap: 1rem;">
                            <div class="form-group">
                                <label>Ville *</label>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" required></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Code Postal *</label>
                                <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" required></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>Pays *</label>
                            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Maroc" Selected="True">Maroc</asp:ListItem>
                                <asp:ListItem Value="France">France</asp:ListItem>
                                <asp:ListItem Value="Belgique">Belgique</asp:ListItem>
                                <asp:ListItem Value="Canada">Canada</asp:ListItem>
                                <asp:ListItem Value="Autre">Autre</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>Téléphone *</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" TextMode="Phone" required></asp:TextBox>
                        </div>
                    </div>

                    <div class="card" style="margin-top: 2rem;">
                        <h2 class="section-title"><i class="fas fa-credit-card"></i> Mode de Paiement</h2>
                        
                        <div class="payment-method selected" id="paymentMethodCod">
                            <label style="cursor: pointer; display: flex; align-items: center; gap: 1rem;">
                                <input type="radio" name="paymentMethod" value="cod" checked style="width: auto;" onchange="toggleBankFields()" />
                                <div>
                                    <strong>Paiement à la livraison</strong>
                                    <p style="margin: 0; color: var(--text-light); font-size: 0.9rem;">Payez en espèces à la réception</p>
                                </div>
                            </label>
                        </div>

                        <div class="payment-method" id="paymentMethodBank">
                            <label style="cursor: pointer; display: flex; align-items: center; gap: 1rem;">
                                <input type="radio" name="paymentMethod" value="bank" style="width: auto;" onchange="toggleBankFields()" />
                                <div>
                                    <strong>Virement bancaire</strong>
                                    <p style="margin: 0; color: var(--text-light); font-size: 0.9rem;">Virement bancaire direct</p>
                                </div>
                            </label>
                        </div>

                        <div id="bankTransferFields" class="bank-transfer-fields">
                            <h4 style="margin-bottom: 1rem; color: var(--text-dark); font-size: 1.1rem;">
                                <i class="fas fa-university"></i> Informations de virement bancaire
                            </h4>
                            
                            <div class="form-group">
                                <label>Nom de la banque *</label>
                                <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" placeholder="Ex: Banque Populaire, Attijariwafa bank"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label>Numéro de compte bancaire (IBAN/RIB) *</label>
                                <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder="Ex: MA64 1234 5678 9012 3456 7890 123"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label>Nom du titulaire du compte *</label>
                                <asp:TextBox ID="txtAccountHolder" runat="server" CssClass="form-control" placeholder="Ex: Ahmed BENALI"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label>Numéro de référence de virement (optionnel)</label>
                                <asp:TextBox ID="txtTransferReference" runat="server" CssClass="form-control" placeholder="Numéro de référence si disponible"></asp:TextBox>
                            </div>

                            <div style="padding: 1rem; background: #fff3cd; border: 1px solid #ffc107; border-radius: 5px; margin-top: 1rem;">
                                <p style="margin: 0; color: #856404; font-size: 0.9rem;">
                                    <i class="fas fa-info-circle"></i> <strong>Note importante :</strong> Veuillez effectuer le virement avant de confirmer la commande. 
                                    Votre commande sera traitée une fois le virement confirmé.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Summary -->
                <div class="order-summary">
                    <h3 style="margin-bottom: 1.5rem;">Récapitulatif</h3>
                    
                    <!-- Promo Code Section -->
                    <div class="promo-code-section" style="margin-bottom: 1.5rem; padding: 1.5rem; background: linear-gradient(135deg, #f8fafc 0%, #ffffff 100%); border-radius: 12px; border: 2px dashed #cbd5e1;">
                        <h4 style="margin-bottom: 1rem; color: #1e293b; font-size: 1rem; display: flex; align-items: center; gap: 0.5rem;">
                            <i class="fas fa-ticket-alt" style="color: #3b82f6;"></i>
                            Code Promo
                        </h4>
                        <asp:Panel ID="pnlPromoCodeSuccess" runat="server" Visible="false" CssClass="promo-success" style="padding: 0.75rem; background: rgba(16, 185, 129, 0.1); border: 1px solid rgba(16, 185, 129, 0.3); border-radius: 8px; margin-bottom: 1rem; color: #059669;">
                            <i class="fas fa-check-circle"></i>
                            <asp:Literal ID="litPromoSuccess" runat="server"></asp:Literal>
                        </asp:Panel>
                        <asp:Panel ID="pnlPromoCodeError" runat="server" Visible="false" CssClass="promo-error" style="padding: 0.75rem; background: rgba(239, 68, 68, 0.1); border: 1px solid rgba(239, 68, 68, 0.3); border-radius: 8px; margin-bottom: 1rem; color: #dc2626;">
                            <i class="fas fa-exclamation-circle"></i>
                            <asp:Literal ID="litPromoError" runat="server"></asp:Literal>
                        </asp:Panel>
                        <div style="display: flex; gap: 0.5rem;">
                            <asp:TextBox ID="txtPromoCode" runat="server" CssClass="form-control" placeholder="Entrez votre code promo" 
                                style="flex: 1; text-transform: uppercase; font-weight: 600; letter-spacing: 1px; font-family: 'Courier New', monospace;"></asp:TextBox>
                            <asp:Button ID="btnApplyPromo" runat="server" Text="Appliquer" CssClass="btn" 
                                style="background: linear-gradient(135deg, #3b82f6, #8b5cf6); color: white; border: none; padding: 0.75rem 1.5rem; border-radius: 8px; font-weight: 600; cursor: pointer; transition: all 0.3s; white-space: nowrap;"
                                OnClick="btnApplyPromo_Click" />
                        </div>
                        <small style="color: #94a3b8; margin-top: 0.5rem; display: block; font-size: 0.85rem;">
                            <i class="fas fa-info-circle"></i> Appliquez un code promo pour bénéficier d'une réduction
                        </small>
                    </div>
                    <div style="margin-bottom: 1.5rem;">
                        <asp:Repeater ID="rptSummary" runat="server">
                            <ItemTemplate>
                                <div class="summary-row">
                                    <span class="summary-item">
                                        <%# Eval("Quantity") %>x <%# Eval("Name") %>
                                    </span>
                                    <span class="summary-price">
                                        <%# Eval("TotalPrice", "{0:F2}") %> MAD
                                    </span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="summary-row">
                        <span>Sous-total</span>
                        <span class="summary-price"><asp:Label ID="lblSubTotal" runat="server"></asp:Label> MAD</span>
                    </div>
                    <div class="summary-row">
                        <span>Livraison</span>
                        <span class="summary-price"><asp:Label ID="lblShipping" runat="server"></asp:Label></span>
                    </div>
                    <asp:Panel ID="pnlDiscount" runat="server" Visible="false" CssClass="summary-row" style="color: #059669;">
                        <span><i class="fas fa-tag"></i> Réduction (<asp:Literal ID="litPromoCodeDisplay" runat="server"></asp:Literal>)</span>
                        <span class="summary-price" style="color: #059669; font-weight: 700;">
                            -<asp:Label ID="lblDiscount" runat="server"></asp:Label> MAD
                        </span>
                    </asp:Panel>
                    <div class="summary-row total-row">
                        <span>Total</span>
                        <span><asp:Label ID="lblTotal" runat="server"></asp:Label> MAD</span>
                    </div>

                    <asp:Button ID="btnPlaceOrder" runat="server" Text="Confirmer la commande" CssClass="btn btn-primary"
                        Style="width: 100%; padding: 15px; font-size: 16px; margin-top: 1.5rem;" 
                        OnClick="btnPlaceOrder_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        function toggleBankFields() {
            var bankFields = document.getElementById('bankTransferFields');
            var codRadio = document.querySelector('input[name="paymentMethod"][value="cod"]');
            var bankRadio = document.querySelector('input[name="paymentMethod"][value="bank"]');
            var codMethod = document.getElementById('paymentMethodCod');
            var bankMethod = document.getElementById('paymentMethodBank');

            if (bankRadio && bankRadio.checked) {
                bankFields.classList.add('show');
                codMethod.classList.remove('selected');
                bankMethod.classList.add('selected');
            } else {
                bankFields.classList.remove('show');
                codMethod.classList.add('selected');
                bankMethod.classList.remove('selected');
            }
        }

        // Initialize on page load
        document.addEventListener('DOMContentLoaded', function() {
            toggleBankFields();
        });
    </script>
</asp:Content>
