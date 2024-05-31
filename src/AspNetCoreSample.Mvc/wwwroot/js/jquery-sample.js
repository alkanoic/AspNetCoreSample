$(document).on("click", ".sample-button", async function () {
  try {
    const response = await fetch(`/jquery/SampleApi`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ text: $(this).siblings(".sample-input").val() }),
    });
    if (response.ok) {
      const r = await response.json();
      $(this)
        .siblings(".sample-label")
        .text(r.text + " " + r.result);
    }
  } catch (error) {
    console.log(error);
  }
});
