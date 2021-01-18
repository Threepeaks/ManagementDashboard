SELECT
           cpr_ref as 'Customer Reference',
           cpr_name as 'Customer',
           cpr_shortname  as 'Abbriviated Name',
           ifnull(cpr_hyphem_trans_code,'[NOTCREATED]') as 'Transaction Code',
           case cpr_loaded_at_hyphen
                when 0 then 'Not Sent for Loading'
                when 1 then 'Waiting on Confirmation'
                else cpr_loaded_at_hyphen
           end as 'State'
           
         FROM
           `tbl_customer_profile`
where cpr_loaded_at_hyphen <> 2
order by cpr_loaded_at_hyphen, cpr_ref