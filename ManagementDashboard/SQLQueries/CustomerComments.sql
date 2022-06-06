
select 
cc_dateentry as 'Entry Date',
cc_comref as 'Customer Reference',
cc_rbr as 'RBR',
cc_debtorreference as 'Debtor Reference',
cc_comment as 'Comment',
ccc_name as 'Category',
usr_username as 'User'


from tblcustomercomments
left join tblcustomercomments_category on cc_categoryid = ccc_id
left join tblusers on usr_id = cc_user_id

order by cc_dateentry desc

limit 50

