# 🐾 FurryFriends Aggregate Review Workflow

Feature-aware, constitution-aligned, and rubric-powered.

This workflow performs a **deep aggregate design review** using:

- The FurryFriends Aggregate Design Rubric

- The Domain Modeling Checklist

- Constitution rules

- Feature folder resolution via `.specify/feature.json`

- Optional manual `feature_dir` override

- Changed-file detection

- Multi-file support

It produces a **severity-scored, rubric-based evaluation** of aggregate correctness.

---

## 🧩 Workflow Definition

```yaml
name: aggregate_review_pipeline

description: Deep aggregate design review using the FurryFriends Aggregate Design Rubric.

inputs:

code:
    type: file

    multiple: true

    required: false

changed_files:
    type: file

    multiple: true

    required: false

feature_dir:
    type: text

    required: false

feature_json:
    type: file

    default: .specify/feature.json

constitution:
    type: file

    default: constitution.md

domain_checklist:
    type: file

    default: .kilocode/docs/domain-modeling-checklist.md

aggregate_rubric:
    type: file

    default: .kilocode/docs/aggregate-design-rubric.md

steps:
    - id: aggregate_review

      skill: aggregate_review

      inputs:
          code: "{{ inputs.code }}"

          changed_files: "{{ inputs.changed_files }}"

          feature_dir: "{{ inputs.feature_dir }}"

          feature_json: "{{ inputs.feature_json }}"

          constitution: "{{ inputs.constitution }}"

          domain_checklist: "{{ inputs.domain_checklist }}"

          aggregate_rubric: "{{ inputs.aggregate_rubric }}"
```
