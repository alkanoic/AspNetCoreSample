import { Modal } from 'bootstrap';
import { LitElement, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';

@customElement('bs-dialog-component')
export class BsDialogComponent extends LitElement {
  protected createRenderRoot() {
    return this;
  }

  @property()
  dialogId?: string;

  public close() {
    const dialog = new Modal(this.querySelector(`#${this.dialogId}`)!);
    dialog.hide();
  }

  render() {
    return html`
      <div
        class="modal fade"
        id="${this.dialogId}"
        tabindex="-1"
        aria-labelledby="${this.dialogId}-label"
        aria-hidden="true"
      >
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title" id="${this.dialogId}-label">Dialog Title</h5>
              <button
                type="button"
                class="btn-close"
                data-bs-dismiss="modal"
                aria-label="Close"
              ></button>
            </div>
            <div class="modal-body">
              <p>Dialog content goes here.</p>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
              <button type="button" class="btn btn-primary" @click="${this.close}">
                Save changes
              </button>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
