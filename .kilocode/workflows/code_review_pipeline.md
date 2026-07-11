\# FurryFriends Multi‑Agent Code Review Pipeline  

Feature‑aware, SpecKit‑integrated, and checklist‑powered.



This workflow performs a complete multi‑agent code review for the FurryFriends architecture.  

It automatically detects the active feature folder via `.specify/feature.json`, unless the user manually overrides it.



```yaml

name: code\_review\_pipeline

description: Multi-agent review for FurryFriends architecture.



inputs:

&#x20; code:

&#x20;   type: file

&#x20;   multiple: true

&#x20;   required: false

&#x20; changed\_files:

&#x20;   type: file

&#x20;   multiple: true

&#x20;   required: false

&#x20; feature\_dir:

&#x20;   type: text

&#x20;   required: false

&#x20; feature\_json:

&#x20;   type: file

&#x20;   default: .specify/feature.json

&#x20; constitution:

&#x20;   type: file

&#x20;   default: constitution.md

&#x20; domain\_checklist:

&#x20;   type: file

&#x20;   default: .kilocode/docs/domain-modeling-checklist.md



steps:

&#x20; - id: correctness

&#x20;   skill: correctness\_review

&#x20;   inputs:

&#x20;     code: "{{ inputs.code }}"

&#x20;     changed\_files: "{{ inputs.changed\_files }}"

&#x20;     feature\_dir: "{{ inputs.feature\_dir }}"

&#x20;     feature\_json: "{{ inputs.feature\_json }}"

&#x20;     domain\_checklist: "{{ inputs.domain\_checklist }}"

&#x20;   save\_as: correctness\_report



&#x20; - id: security

&#x20;   skill: security\_review

&#x20;   inputs:

&#x20;     code: "{{ inputs.code }}"

&#x20;     changed\_files: "{{ inputs.changed\_files }}"

&#x20;     feature\_dir: "{{ inputs.feature\_dir }}"

&#x20;     feature\_json: "{{ inputs.feature\_json }}"

&#x20;     domain\_checklist: "{{ inputs.domain\_checklist }}"

&#x20;   save\_as: security\_report



&#x20; - id: constitution

&#x20;   skill: constitution\_review

&#x20;   inputs:

&#x20;     code: "{{ inputs.code }}"

&#x20;     changed\_files: "{{ inputs.changed\_files }}"

&#x20;     feature\_dir: "{{ inputs.feature\_dir }}"

&#x20;     feature\_json: "{{ inputs.feature\_json }}"

&#x20;     constitution: "{{ inputs.constitution }}"

&#x20;     domain\_checklist: "{{ inputs.domain\_checklist }}"

&#x20;   save\_as: constitution\_report



&#x20; - id: aggregate

&#x20;   skill: aggregate\_review

&#x20;   inputs:

&#x20;     code: "{{ inputs.code }}"

&#x20;     changed\_files: "{{ inputs.changed\_files }}"

&#x20;     feature\_dir: "{{ inputs.feature\_dir }}"

&#x20;     feature\_json: "{{ inputs.feature\_json }}"

&#x20;     constitution: "{{ inputs.constitution }}"

&#x20;     domain\_checklist: "{{ inputs.domain\_checklist }}"

&#x20;     correctness\_report: "{{ steps.correctness.output }}"

&#x20;     security\_report: "{{ steps.security.output }}"

&#x20;     constitution\_report: "{{ steps.constitution.output }}"



