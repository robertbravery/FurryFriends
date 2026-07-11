\# 🐾 FurryFriends Aggregate Review Workflow  

Feature-aware, constitution-aligned, and rubric-powered.



This workflow performs a \*\*deep aggregate design review\*\* using:



\- The FurryFriends Aggregate Design Rubric  

\- The Domain Modeling Checklist  

\- Constitution rules  

\- Feature folder resolution via `.specify/feature.json`  

\- Optional manual `feature\_dir` override  

\- Changed-file detection  

\- Multi-file support  



It produces a \*\*severity-scored, rubric-based evaluation\*\* of aggregate correctness.



\---



\## 🧩 Workflow Definition



```yaml

name: aggregate\_review\_pipeline

description: Deep aggregate design review using the FurryFriends Aggregate Design Rubric.



inputs:

code:

  type: file

  multiple: true

  required: false



changed\_files:

  type: file

  multiple: true

  required: false



feature\_dir:

  type: text

  required: false



feature\_json:

  type: file

  default: .specify/feature.json



constitution:

  type: file

  default: constitution.md



domain\_checklist:

  type: file

  default: .kilocode/docs/domain-modeling-checklist.md



aggregate\_rubric:

  type: file

  default: .kilocode/docs/aggregate-design-rubric.md



steps:

- id: aggregate\_review

  skill: aggregate\_review

  inputs:

    code: "{{ inputs.code }}"

    changed\_files: "{{ inputs.changed\_files }}"

    feature\_dir: "{{ inputs.feature\_dir }}"

    feature\_json: "{{ inputs.feature\_json }}"

    constitution: "{{ inputs.constitution }}"

    domain\_checklist: "{{ inputs.domain\_checklist }}"

    aggregate\_rubric: "{{ inputs.aggregate\_rubric }}"



