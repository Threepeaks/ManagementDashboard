select com_ref as 'Customer Reference',
if(com_ac_pending=1, com_ac_pending_date,'') as 'Pending Start Date',
if(com_ac_pending=1, com_acc_pending_flagdate,'') as 'Pending End Date'

,if(com_acc_cancel = 1,com_acc_cancel_date,'') as 'Cancel Start Date'
,if(com_acc_cancel = 1,com_acc_cancel_enddate,'') as 'Cancel End Date'

,fmf_amount as 'Management Fee',
case concat(com_ac_pending,com_acc_cancel)
when '00' then 'Active'
when '10' then 'Pending'
when '01' then 'In Cancellation'
when '11' then 'Pending/In Canellation'

else concat(com_ac_pending,com_acc_cancel)
end as 'State',
case concat(com_ac_pending,com_acc_cancel)
when '00' then fmf_amount
when '10' then if(com_acc_pending_flagdate < (date_add(LAST_DAY(CURDATE()),interval 1 DAY)),fmf_amount,0)
when '01' then if(com_acc_cancel_enddate < (date_add(LAST_DAY(CURDATE()),interval 1 DAY)),0,fmf_amount)
when '11' then 'Pending/In Canellation'

else concat(com_ac_pending,com_acc_cancel)
end as 'Prediction Amount',
concat(com_ac_pending,com_acc_cancel) as 'P'

from tblcompany
LEFT JOIN `tblfee_monthlyfee` ON `tblcompany`.`com_fee_monthly` = `tblfee_monthlyfee`.`fmf_id`
where com_acc_cancel != 2


-- group by com_ref WITH ROLLUP




union


select '',NULL,NULL,NULL,NULL,NULL,'Total',
CAST(sum(case concat(com_ac_pending,com_acc_cancel)
when '00' then fmf_amount
when '10' then if(com_acc_pending_flagdate < (date_add(LAST_DAY(CURDATE()),interval 1 DAY)),fmf_amount,0)
when '01' then if(com_acc_cancel_enddate < (date_add(LAST_DAY(CURDATE()),interval 1 DAY)),0,fmf_amount)
when '11' then 'Pending/In Canellation'

else concat(com_ac_pending,com_acc_cancel)
end) AS DECIMAL(12,2)) ,-1

from tblcompany
LEFT JOIN `tblfee_monthlyfee` ON `tblcompany`.`com_fee_monthly` = `tblfee_monthlyfee`.`fmf_id`

where com_acc_cancel != 2

order by p desc, 'Customer Reference'
