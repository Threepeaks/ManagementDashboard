SELECT cpr_referred_by as 'Referred By', 
count(*) as 'Count'
FROM threepeaks_tpms.tbl_customer_profile group by cpr_referred_by