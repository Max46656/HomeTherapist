
<?php
    $column['value'] = $column['value'] ?? data_get($entry, $column['name']);
    $column['escaped'] = $column['escaped'] ?? true;
    $column['prefix'] = $column['prefix'] ?? '';
    $column['suffix'] = $column['suffix'] ?? '';

    if($column['value'] instanceof \Closure) {
        $column['value'] = $column['value']($entry);
    }

    if (in_array($column['value'], [true, 1, '1'])) {
        $related_key = 1;
        if ( isset( $column['options'][1] ) ) {
            $column['text'] = $column['options'][1];
            $column['escaped'] = false;
        } else {
            $column['text'] = Lang::has('backpack::crud.yes') ? trans('backpack::crud.yes') : 'Yes';
        }
    } else {
        $related_key = 0;
        if ( isset( $column['options'][0] ) ) {
            $column['text'] = $column['options'][0];
            $column['escaped'] = false;
        } else {
            $column['text'] = Lang::has('backpack::crud.no') ? trans('backpack::crud.no') : 'No';
        }
    }

    $column['text'] = $column['prefix'].$column['text'].$column['suffix'];
?>

<span data-order="<?php echo e($column['value']); ?>">
    <?php echo $__env->renderWhen(!empty($column['wrapper']), 'crud::columns.inc.wrapper_start', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?>
        <?php if($column['escaped']): ?>
            <?php echo e($column['text']); ?>

        <?php else: ?>
            <?php echo $column['text']; ?>

        <?php endif; ?>
    <?php echo $__env->renderWhen(!empty($column['wrapper']), 'crud::columns.inc.wrapper_end', \Illuminate\Support\Arr::except(get_defined_vars(), ['__data', '__path'])); ?>
</span>
<?php /**PATH C:\xampp8.2.0\htdocs\HomeTherapist\HomeTherapistAdmin\vendor\backpack\crud\src\resources\views\crud/columns/boolean.blade.php ENDPATH**/ ?>