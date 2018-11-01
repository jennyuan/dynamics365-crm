// JavaScript source code
showHideSection =
{
    showHideVendorSection: function () {         // Value of the Customer Type field         
        var customerType = Xrm.Page.getAttribute("customertypecode").getValue();
        // Vendor section of the form         
        var vendorSection = Xrm.Page.getControl("moc_vendorcode").getParent();
        // Check if the Customer Type is a Vendor.         
        // The Customer Type field is an Option Set and the value for Vendor is 11         
        if (customerType == 11) {
            // If the Customer is a Vendor, make the Vendor section visible           
            vendorSection.setVisible(true);
        }
        else {
            // If the Customer is not a Vendor, hide the Vendor section of the form           
            vendorSection.setVisible(false);
        }
    }
}