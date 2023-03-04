#! /usr/bin/env sh
docs=$(ls ./coveragereport | grep .htm)

# Delete previous report
curl -X "DELETE" -H "Token: ${TOKEN}" ${REPORT_URI}/${BRANCH_NAME}

# Send new report files
for doc in $docs; do
	echo "Sending $doc..."
	curl -H "Token: ${TOKEN}" -H "Content-Type: text/html" -H "X-File-Name: ${doc}" -X POST --data "@./coveragereport/$doc" ${REPORT_URI}/${BRANCH_NAME}
	rm "./coveragereport/$doc"
done
