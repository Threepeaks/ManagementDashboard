select 

com_ref as 'Account Reference',
(select sum(tbl_accounting_depost_tracking.amount)   from tbl_accounting_depost_tracking where tbl_accounting_depost_tracking.cref = com_ref and tbl_accounting_depost_tracking.deposit_date <= '{endDate}') as 'Deposit',
(select sum(rbr_h_amnt_ok) from tblrbr where rbr_comref  = com_ref and rbr_date between '{startDate}' and '{endDate}') as 'Collection',



ifnull(round((((select sum(tbl_accounting_depost_tracking.amount)   from tbl_accounting_depost_tracking where tbl_accounting_depost_tracking.cref = com_ref and tbl_accounting_depost_tracking.deposit_date <= '{endDate}') /
(select sum(rbr_h_amnt_ok) from tblrbr where rbr_comref  = com_ref and rbr_date between '{startDate}' and '{endDate}')) * 100),2),0) as 'Deposit Ratio',


(select sum(dbt_amount) from tbldebits where dbt_comref = com_ref and dbt_pass_unpaid in (2,3) and dbt_unpaid_datetime  between '{startDate}' and '{endDate}') as 'Unpaids',

round(
ifnull(
((select sum(dbt_amount) from tbldebits where dbt_comref = com_ref and dbt_pass_unpaid in (2,3) and dbt_unpaid_datetime  between '{startDate}' and '{endDate}') / 
(select sum(rbr_h_amnt_ok) from tblrbr where rbr_comref  = com_ref and rbr_date between '{startDate}' and '{endDate}') ) * 100 ,0),2) as 'Unp Ratio',

if ((select sum(tbl_accounting_depost_tracking.amount)   from tbl_accounting_depost_tracking where tbl_accounting_depost_tracking.cref = com_ref and tbl_accounting_depost_tracking.deposit_date <= '{endDate}')  > 
ifnull((select sum(dbt_amount) from tbldebits where dbt_comref = com_ref and dbt_pass_unpaid in (2,3) and dbt_unpaid_datetime  between '{startDate}' and '{endDate}'),0), "Yes","No") as 'Is Covered'


from tblcompany 

where com_retterms = 3
and com_acc_cancel in (0,1)

order by com_ref