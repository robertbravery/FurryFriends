# FurryFriends Multi‑Agent Code Review Pipeline

Feature‑aware, SpecKit‑integrated, and checklist‑powered.

This workflow performs a complete multi‑agent code review for the FurryFriends architecture.

It automatically detects the active feature folder via `.specify/feature.json`, unless the user manually overrides it.

```yaml
name: code_review_pipeline

description: Multi-agent review for FurryFriends architecture.

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

steps:
    - id: correctness

      skill: correctness_review

      inputs:
          code: "{{ inputs.code }}"

          changed_files: "{{ inputs.changed_files }}"

          feature_dir: "{{ inputs.feature_dir }}"

          feature_json: "{{ inputs.feature_json }}"

          domain_checklist: "{{ inputs.domain_checklist }}"

      save_as: correctness_report

    - id: security

      skill: security_review

      inputs:
          code: "{{ inputs.code }}"

          changed_files: "{{ inputs.changed_files }}"

          feature_dir: "{{ inputs.feature_dir }}"

          feature_json: "{{ inputs.feature_json }}"

          domain_checklist: "{{ inputs.domain_checklist }}"

      save_as: security_report

    - id: constitution

      skill: constitution_review

      inputs:
          code: "{{ inputs.code }}"

          changed_files: "{{ inputs.changed_files }}"

          feature_dir: "{{ inputs.feature_dir }}"

          feature_json: "{{ inputs.feature_json }}"

          constitution: "{{ inputs.constitution }}"

          domain_checklist: "{{ inputs.domain_checklist }}"

      save_as: constitution_report

    - id: aggregate

      skill: aggregate_review

      inputs:
          code: "{{ inputs.code }}"

          changed_files: "{{ inputs.changed_files }}"

          feature_dir: "{{ inputs.feature_dir }}"

          feature_json: "{{ inputs.feature_json }}"

          constitution: "{{ inputs.constitution }}"

          domain_checklist: "{{ inputs.domain_checklist }}"

          correctness_report: "{{ steps.correctness.output }}"

          security_report: "{{ steps.security.output }}"

          constitution_report: "{{ steps.constitution.output }}"
```
