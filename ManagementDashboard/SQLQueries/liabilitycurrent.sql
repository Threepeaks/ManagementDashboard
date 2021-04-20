
select 
                com_ref  as 'CREF'
   , case concat(com_acc_cancel,com_ac_pending) 
                                when '00' then 'Active'
        when '01' then 'Pending'
        when '10' then 'In Cancellation'
        when '11' then 'In Cancellation'
        when '20' then 'Cancelled'
       when '21' then 'Cancelled'
        else concat(com_acc_cancel,com_ac_pending)
    end as 'Status',
    if(ifnull(f.customer,'Hyphen')='Hyphen','Hyphen','Fulcrum') as 'Gateway'

    
                ,ifnull((select rcd_cbal from tblrecon_data where rcd_comref=com_ref order by rcd_id desc limit 1),0) as 'BCF'
                ,ifnull((select sum(rbr_h_amnt_ok) from tblrbr where rbr_status = 2 and rbr_comref=com_ref and rbr_date <= date_add( NOW(),interval -1 day)),0) as 'Collection'
                ,ifnull((select sum(dbt_amount) from tbldebits where dbt_comref=com_ref and dbt_pass_unpaid in (2) and dbt_unpaid_remid = 0),0) as 'Unpaids'
                ,ifnull((select  sum(dbt_amount) from tbldebits where dbt_comref=com_ref and dbt_pass_unpaid in (3) and dbt_lunpaid_remid = 0),0) as 'LateUnpaids'
                ,ifnull((select sum(rbr_total_retention) from tblrbr where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status =3),0) as 'RetentionHeld'
                ,ifnull((select sum(dta_amount) from tbldebtors_account where dta_cref=com_ref and dta_status=0 ),0) as 'Account'



   
 
 from tblcompany 
 left join tbl_gateway_fulcrum_client f on f.customer = com_ref order by com_ref
  -- limit 10
;

