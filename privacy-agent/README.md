Training material. Invented data.

# privacy-agent

This folder holds the GDPR request intake agent of cycle 6. The agent is NOT written yet:
participants write it during the training with the Claude Agent SDK (Python, in all three
starter repositories).

What is here:

- `policy.md`: the GDPR intake policy of Stejar Bank that the agent must follow.
- `requests/`: 8 synthetic GDPR requests (JSON, each with `"synthetic": true`).
- `requirements.txt`: the Python dependency of the agent (`claude-agent-sdk`).
- `output/`: where the agent writes its triage results and its audit log.

Install: the setup script of the repository creates `privacy-agent/.venv` and installs
`requirements.txt` in it. To do it by hand, from the root of the repository:

```
# Windows (PowerShell)
py -3 -m venv privacy-agent\.venv
privacy-agent\.venv\Scripts\python -m pip install -r privacy-agent\requirements.txt
# macOS
python3 -m venv privacy-agent/.venv
privacy-agent/.venv/bin/python -m pip install -r privacy-agent/requirements.txt
```

Rules for the agent (from `policy.md`): it classifies and routes requests, it never sends a
reply to a requester, and it never reads customer data without human approval.
