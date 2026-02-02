The mini project of Virtual Machine Rebuild API
1. takes the POST /api/v1/rebuild request with parameters (vmName, template)
2. runes rebuild as power shell task
3. returns jobId
4. allows to check status by endpoint GET /api/v1/jobs/{jobId}
5. logs


Structure:
VmRebuildApi/
  Controllers/
  Jobs/
  Models/
  Services/
  Scripts/